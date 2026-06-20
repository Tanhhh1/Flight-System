using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.CQRS.SeatReserve.Commands.HoldSeat
{
    public class HoldSeatHandler : IRequestHandler<HoldSeatCommand, ApiResult<HoldSeatDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly ISeatNotificationService _seatNotification;
        private const int LockMinutes = 10;

        public HoldSeatHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, ISeatNotificationService seatNotification)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _seatNotification = seatNotification;
        }

        public async Task<ApiResult<HoldSeatDto>> Handle(HoldSeatCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.Id;
            if (userId == null)
                return ApiResult<HoldSeatDto>.Failure("Bạn chưa đăng nhập.");

            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId && b.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
                return ApiResult<HoldSeatDto>.Failure("Booking không tồn tại.");

            var bookingDetail = await _unitOfWork.BookingDetailRepository
                .GetByCondition(bd =>
                    bd.BookingId == request.BookingId &&
                    bd.BookingFlightId == request.FlightId &&
                    bd.PassengerId == request.PassengerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (bookingDetail == null)
                return ApiResult<HoldSeatDto>.Failure("Hành khách hoặc chuyến bay không thuộc booking này.");

            var seatTemplate = await _unitOfWork.SeatTemplateRepository
                .GetByCondition(st => st.SeatId == request.SeatId)
                .Include(st => st.SeatClass)
                .FirstOrDefaultAsync(cancellationToken);

            if (seatTemplate == null)
                return ApiResult<HoldSeatDto>.Failure("Ghế không tồn tại.");

            if (seatTemplate.ClassId != booking.ClassId)
                return ApiResult<HoldSeatDto>.Failure($"Ghế {seatTemplate.SeatNumber} thuộc {seatTemplate.SeatClass.ClassName}, " + $"không khớp với hạng vé của bạn.");

            var existingFlightSeat = await _unitOfWork.FlightSeatRepository
                .GetByCondition(fs =>
                    fs.FlightId == request.FlightId &&
                    fs.SeatId == request.SeatId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingFlightSeat != null)
            {
                if (existingFlightSeat.Status == SeatStatus.Booked)
                    return ApiResult<HoldSeatDto>.Failure("Ghế này đã được đặt.");

                if (existingFlightSeat.Status == SeatStatus.Locked &&
                    existingFlightSeat.LockedUntil > DateTime.UtcNow &&
                    existingFlightSeat.LockedBy != request.PassengerId)
                    return ApiResult<HoldSeatDto>.Failure("Ghế này đang được người khác giữ.");
            }

            try
            {
                var flightSeat = new FlightSeat
                {
                    FlightId = request.FlightId,
                    SeatId = request.SeatId,
                    Status = SeatStatus.Locked,
                    LockedBy = request.PassengerId,
                    LockedUntil = DateTime.UtcNow.AddMinutes(LockMinutes),
                };

                await _unitOfWork.FlightSeatRepository.AddAsync(flightSeat);
                await _unitOfWork.SaveChangesAsync();

                _ = _seatNotification.NotifySeatLockedAsync(
                    request.FlightId,
                    flightSeat.FlightSeatId,
                    flightSeat.SeatId,
                    request.PassengerId);

                return ApiResult<HoldSeatDto>.Success(new HoldSeatDto
                {
                    FlightSeatId = flightSeat.FlightSeatId,
                    FlightId = flightSeat.FlightId,
                    SeatId = flightSeat.SeatId,
                    SeatNumber = seatTemplate.SeatNumber,
                    PassengerId = request.PassengerId,
                    LockedUntil = flightSeat.LockedUntil,
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                return ApiResult<HoldSeatDto>.Failure(
                    "Ghế vừa được người khác chọn, vui lòng chọn ghế khác.");
            }
            catch (DbUpdateException)
            {
                return ApiResult<HoldSeatDto>.Failure(
                    "Ghế vừa được người khác chọn, vui lòng chọn ghế khác.");
            }
        }
    }
}
