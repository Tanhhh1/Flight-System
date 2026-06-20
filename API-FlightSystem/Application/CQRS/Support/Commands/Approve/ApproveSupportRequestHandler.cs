using Application.Common;
using Application.CQRS.Support.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Support.Commands.Approve
{
    public class ApproveSupportRequestHandler : IRequestHandler<ApproveSupportRequestCommand, ApiResult<SupportRequestDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApproveSupportRequestHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<SupportRequestDto>> Handle(ApproveSupportRequestCommand request, CancellationToken cancellationToken)
        {
            var supportRequest = await _unitOfWork.SupportRequestRepository
                    .GetByCondition(sr => sr.RequestId == request.RequestId)
                    .Include(sr => sr.Booking).ThenInclude(b => b.BookingDetails)
                    .FirstOrDefaultAsync(cancellationToken);

            if (supportRequest is null)
                return ApiResult<SupportRequestDto>.Failure("Không tìm thấy yêu cầu hỗ trợ");

            if (supportRequest.Status != SupportStatus.Pending)
                return ApiResult<SupportRequestDto>.Failure("Chỉ có thể duyệt yêu cầu đang chờ xử lý");

            if (supportRequest.Booking.Status != BookingStatus.Confirmed)
                return ApiResult<SupportRequestDto>.Failure("Đơn đặt vé không hợp lệ");

            if (supportRequest.RequestType == RequestType.Refund)
                await HandleRefundAsync(supportRequest, cancellationToken);
            else
                await HandleRescheduleAsync(supportRequest, cancellationToken);

            supportRequest.Status = SupportStatus.Approved;
            _unitOfWork.SupportRequestRepository.Update(supportRequest);

            var dto = supportRequest.Adapt<SupportRequestDto>();
            return ApiResult<SupportRequestDto>.Success(dto);
        }

        private async Task HandleRefundAsync(SupportRequest supportRequest, CancellationToken cancellationToken)
        {
            var flightIds = supportRequest.Booking.BookingDetails
                .Select(bd => bd.BookingFlightId).Distinct().ToList();

            var passengerCountPerFlight = supportRequest.Booking.BookingDetails
                .GroupBy(bd => bd.BookingFlightId)
                .ToDictionary(g => g.Key, g => g.Count());

            var seatPrices = await _unitOfWork.FlightSeatPriceRepository
                .GetByCondition(fsp => flightIds.Contains(fsp.FlightId) && fsp.ClassId == supportRequest.Booking.ClassId)
                .ToListAsync(cancellationToken);

            foreach (var seatPrice in seatPrices)
            {
                seatPrice.AvailableSeats += passengerCountPerFlight[seatPrice.FlightId];
                _unitOfWork.FlightSeatPriceRepository.Update(seatPrice);
            }
            supportRequest.Booking.Status = BookingStatus.Cancelled;
            _unitOfWork.BookingRepository.Update(supportRequest.Booking);
        }

        private async Task HandleRescheduleAsync(SupportRequest supportRequest, CancellationToken cancellationToken)
        {
            var booking = supportRequest.Booking;
            var newFlightId = supportRequest.NewFlightId!.Value;
            var passengerCount = booking.BookingDetails.Count;

            var newSeatPrice = await _unitOfWork.FlightSeatPriceRepository
                .GetByCondition(fsp =>
                    fsp.FlightId == newFlightId &&
                    fsp.ClassId == booking.ClassId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Không tìm thấy giá vé cho chuyến bay mới.");

            if (newSeatPrice.AvailableSeats < passengerCount)
                throw new Exception("Chuyến bay mới không còn đủ chỗ trống.");

            var oldFlightId = booking.BookingDetails.Select(bd => bd.BookingFlightId).First();
            var oldSeatPrice = await _unitOfWork.FlightSeatPriceRepository
                .GetByCondition(fsp => fsp.FlightId == oldFlightId && fsp.ClassId == booking.ClassId)
                .FirstOrDefaultAsync(cancellationToken);

            foreach (var detail in booking.BookingDetails)
            {
                if (detail.FlightSeatId.HasValue)
                {
                    var flightSeat = await _unitOfWork.FlightSeatRepository
                        .GetByCondition(fs => fs.FlightSeatId == detail.FlightSeatId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (flightSeat is not null)
                        _unitOfWork.FlightSeatRepository.Delete(flightSeat);

                    detail.FlightSeatId = null;
                }
                detail.BookingFlightId = newFlightId;
                _unitOfWork.BookingDetailRepository.Update(detail);
            }
            if (oldSeatPrice is not null)
            {
                oldSeatPrice.AvailableSeats += passengerCount;
                _unitOfWork.FlightSeatPriceRepository.Update(oldSeatPrice);
            }
            newSeatPrice.AvailableSeats -= passengerCount;
            _unitOfWork.FlightSeatPriceRepository.Update(newSeatPrice);
        }
    }
}
