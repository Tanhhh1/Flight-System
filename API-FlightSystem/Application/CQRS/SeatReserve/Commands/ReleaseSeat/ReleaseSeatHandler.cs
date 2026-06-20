using Application.Common;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReserve.Commands.ReleaseSeat
{
    public class ReleaseSeatHandler : IRequestHandler<ReleaseSeatCommand, ApiResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ISeatNotificationService _seatNotification;

        public ReleaseSeatHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, ISeatNotificationService seatNotification)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _seatNotification = seatNotification;
        }

        public async Task<ApiResult<bool>> Handle(ReleaseSeatCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.Id;
            if (userId == null)
                return ApiResult<bool>.Failure("Bạn chưa đăng nhập.");

            var flightSeat = await _unitOfWork.FlightSeatRepository
                .GetByCondition(fs =>
                    fs.FlightSeatId == request.FlightSeatId &&
                    fs.FlightId == request.FlightId &&
                    fs.LockedBy == request.PassengerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (flightSeat == null)
                return ApiResult<bool>.Failure("Ghế không tồn tại hoặc bạn không có quyền bỏ chọn ghế này.");

            if (flightSeat.Status == SeatStatus.Booked)
                return ApiResult<bool>.Failure("Ghế đã được đặt, không thể bỏ chọn.");

            var bookingDetail = await _unitOfWork.BookingDetailRepository
                .GetByCondition(bd =>
                    bd.BookingId == request.BookingId &&
                    bd.BookingFlightId == request.FlightId &&
                    bd.PassengerId == request.PassengerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (bookingDetail == null)
                return ApiResult<bool>.Failure("Hành khách không thuộc booking này.");

            var seatId = flightSeat.SeatId;
            _unitOfWork.FlightSeatRepository.Delete(flightSeat);
            await _unitOfWork.SaveChangesAsync();

            var flightSeatIds = new List<int> { request.FlightSeatId };
            var seatIds = new List<int> { seatId };
            _ = _seatNotification.NotifySeatsReleasedAsync(
                request.FlightId,
                flightSeatIds,
                seatIds);

            return ApiResult<bool>.Success(true);
        }
    }
}
