using Application.Common;
using Application.CQRS.Support.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Support.Commands.Create
{
    public class CreateSupportRequestHandler : IRequestHandler<CreateSupportRequestCommand, ApiResult<SupportRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public CreateSupportRequestHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<SupportRequestDto>> Handle(CreateSupportRequestCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<SupportRequestDto>.Failure("Bạn cần đăng nhập");

            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId && b.UserId == _currentUser.Id)
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(cancellationToken); if (booking is null)
                return ApiResult<SupportRequestDto>.Failure("Không tìm thấy đơn đặt vé");

            if (booking.Status != BookingStatus.Confirmed)
                return ApiResult<SupportRequestDto>.Failure("Chỉ có thể gửi yêu cầu cho đơn đặt vé đã thanh toán");

            var firstFlightId = booking.BookingDetails
                .Select(bd => bd.BookingFlightId)
                .FirstOrDefault();

            if (firstFlightId == 0)
                return ApiResult<SupportRequestDto>.Failure("Không tìm thấy thông tin chuyến bay");

            var flight = await _unitOfWork.FlightRepository
                .GetByCondition(f => f.FlightId == firstFlightId)
                .Include(f => f.Policy)
                .FirstOrDefaultAsync(cancellationToken);

            if (request.RequestType == RequestType.Refund && flight?.Policy.IsRefund == false)
                return ApiResult<SupportRequestDto>.Failure("Vé này không hỗ trợ hoàn tiền");

            if (request.RequestType == RequestType.Reschedule && flight?.Policy.IsChange == false)
                return ApiResult<SupportRequestDto>.Failure("Vé này không hỗ trợ đổi lịch");

            var hasPending = await _unitOfWork.SupportRequestRepository
                .GetByCondition(sr => sr.BookingId == request.BookingId && sr.Status == SupportStatus.Pending)
                .AnyAsync(cancellationToken);

            if (hasPending)
                return ApiResult<SupportRequestDto>.Failure("Đơn đặt vé này đang có yêu cầu chờ xử lý");

            if (request.RequestType == RequestType.Reschedule)
            {
                var newFlight = await _unitOfWork.FlightRepository
                    .GetByCondition(f => f.FlightId == request.NewFlightId)
                    .Include(f => f.Route)
                    .FirstOrDefaultAsync(cancellationToken);

                if (newFlight is null)
                    return ApiResult<SupportRequestDto>.Failure("Chuyến bay mới không tồn tại");

                if (newFlight.RouteId != flight!.RouteId)
                    return ApiResult<SupportRequestDto>.Failure("Chuyến bay mới phải cùng tuyến đường với chuyến bay cũ");

                var passengerCount = await _unitOfWork.BookingDetailRepository
                    .GetByCondition(bd => bd.BookingId == request.BookingId)
                    .CountAsync(cancellationToken);

                var seatPrice = await _unitOfWork.FlightSeatPriceRepository
                    .GetByCondition(fsp =>
                        fsp.FlightId == request.NewFlightId &&
                        fsp.ClassId == booking.ClassId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (seatPrice is null || seatPrice.AvailableSeats < passengerCount)
                    return ApiResult<SupportRequestDto>.Failure("Chuyến bay mới không đủ chỗ trống");
            }

            var supportRequest = new SupportRequest
            {
                BookingId = request.BookingId,
                NewFlightId = request.NewFlightId,
                RequestType = request.RequestType,
                Reason = request.Reason,
                Status = SupportStatus.Pending,
            };

            await _unitOfWork.SupportRequestRepository.AddAsync(supportRequest);
            await _unitOfWork.SaveChangesAsync();

            var dto = supportRequest.Adapt<SupportRequestDto>();
            dto.BookingCode = booking.BookingCode;
            dto.RequestType = request.RequestType.ToString();
            dto.Status = SupportStatus.Pending.ToString();

            return ApiResult<SupportRequestDto>.Success(dto);
        }
    }
}
