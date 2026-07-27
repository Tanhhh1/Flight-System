using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReserve.Queries.Verify
{
    public class VerifyBookingHandler : IRequestHandler<VerifyBookingQuery, ApiResult<VerifyBookingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerifyBookingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<VerifyBookingDto>> Handle(VerifyBookingQuery request, CancellationToken cancellationToken)
        {
            var bookingValidation = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingCode == request.BookingCode)
                .AsNoTracking()
                .Select(b => new { b.Status, HasInvalidFlight = b.BookingDetails.Any(bd => bd.Flight != null &&
                        (bd.Flight.Status == FlightStatus.Completed || bd.Flight.Status == FlightStatus.Cancelled))})
                .FirstOrDefaultAsync(cancellationToken);

            if (bookingValidation is null)
                return ApiResult<VerifyBookingDto>.Failure("Mã đơn vé không hợp lệ");

            if (bookingValidation.Status != BookingStatus.Confirmed)
                return ApiResult<VerifyBookingDto>.Failure("Đơn đặt chỗ chưa được thanh toán hoặc đã bị hủy");

            if (bookingValidation.HasInvalidFlight)
                return ApiResult<VerifyBookingDto>.Failure("Chuyến bay đã hoàn thành hoặc đã bị hủy, không thể thực hiện đặt ghế");

            var dto = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingCode == request.BookingCode)
                .AsNoTracking()
                .ProjectToType<VerifyBookingDto>()
                .FirstOrDefaultAsync(cancellationToken);

            return ApiResult<VerifyBookingDto>.Success(dto!);
        }
    }
}