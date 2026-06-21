using Application.Common;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReserve.Commands.ConfirmSeat
{
    public class ConfirmSeatsHandler : IRequestHandler<ConfirmSeatsCommand, ApiResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ISeatNotificationService _seatNotification;

        public ConfirmSeatsHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, ISeatNotificationService seatNotification)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _seatNotification = seatNotification;
        }

        public async Task<ApiResult<bool>> Handle(ConfirmSeatsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.Id;
            if (userId == null)
                return ApiResult<bool>.Failure("Bạn chưa đăng nhập");

            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId && b.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
                return ApiResult<bool>.Failure("Mã đơn đặt vé không tồn tại");

            var now = DateTime.UtcNow;
            var notifyTasks = new List<(int FlightId, int FlightSeatId, int SeatId)>();

            foreach (var (flightId, assignments) in request.Assignments)
            {
                if (!assignments.Any()) continue;
                var flightSeatIds = assignments.Select(a => a.FlightSeatId).ToList();

                var flightSeats = await _unitOfWork.FlightSeatRepository
                    .GetByCondition(fs => fs.FlightId == flightId && flightSeatIds.Contains(fs.FlightSeatId))
                    .ToListAsync(cancellationToken);

                foreach (var assignment in assignments)
                {
                    var flightSeat = flightSeats
                        .FirstOrDefault(fs => fs.FlightSeatId == assignment.FlightSeatId);

                    if (flightSeat == null)
                        return ApiResult<bool>.Failure("Ghế không còn được giữ, vui lòng chọn lại");

                    if (flightSeat.Status != SeatStatus.Locked)
                        return ApiResult<bool>.Failure("Ghế không ở trạng thái giữ chỗ");

                    if (flightSeat.LockedUntil < now)
                        return ApiResult<bool>.Failure("Phiên giữ ghế đã hết hạn, vui lòng chọn lại");

                    if (flightSeat.LockedBy != assignment.PassengerId)
                        return ApiResult<bool>.Failure("Bạn không có quyền xác nhận ghế này");

                    var bookingDetail = await _unitOfWork.BookingDetailRepository
                        .GetByCondition(bd =>
                            bd.BookingId == request.BookingId &&
                            bd.BookingFlightId == flightId &&
                            bd.PassengerId == assignment.PassengerId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (bookingDetail == null)
                        return ApiResult<bool>.Failure("Hành khách không thuộc đơn đặt vé này");

                    flightSeat.Status = SeatStatus.Booked;
                    flightSeat.LockedBy = 0;
                    flightSeat.LockedUntil = DateTime.MinValue;
                    _unitOfWork.FlightSeatRepository.Update(flightSeat);

                    bookingDetail.FlightSeatId = flightSeat.FlightSeatId;
                    _unitOfWork.BookingDetailRepository.Update(bookingDetail);

                    notifyTasks.Add((flightId, flightSeat.FlightSeatId, flightSeat.SeatId));
                }
            }
            foreach (var (flightId, flightSeatId, seatId) in notifyTasks)
            {
                _ = _seatNotification.NotifySeatBookedAsync(flightId, flightSeatId, seatId);
            }

            return ApiResult<bool>.Success(true);
        }
    }
}
