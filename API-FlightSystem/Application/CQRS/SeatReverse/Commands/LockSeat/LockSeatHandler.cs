using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using Application.Hubs;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace Application.CQRS.SeatReverse.Commands.LockSeat
{
    public class LockSeatHandler : IRequestHandler<LockSeatCommand, ApiResult<LockSeatDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IHubContext<SeatHub, ISeatHubClient> _hubContext;

        public LockSeatHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IHubContext<SeatHub, ISeatHubClient> hubContext)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _hubContext = hubContext;
        }

        public async Task<ApiResult<LockSeatDto>> Handle(LockSeatCommand request, CancellationToken cancellationToken)
        {
            var bookingDetail = await _unitOfWork.BookingDetailRepository
                .GetByCondition(bd => bd.BookingDetailId == request.BookingDetailId && bd.Booking.UserId == _currentUser.Id)
                .Include(bd => bd.Booking)
                .FirstOrDefaultAsync(cancellationToken);

            if (bookingDetail == null)
                return ApiResult<LockSeatDto>.Failure("Không tìm thấy thông tin đặt vé");

            var seat = await _unitOfWork.FlightSeatRepository
                .GetByCondition(s => s.FlightSeatId == request.FlightSeatId && s.FlightId == bookingDetail.FlightId)
                .Include(s => s.SeatTemplate).ThenInclude(st => st.SeatClass)
                .FirstOrDefaultAsync(cancellationToken);

            if (seat == null)
                return ApiResult<LockSeatDto>.Failure("Không tìm thấy ghế");

            if (seat.Status == SeatStatus.Booked)
                return ApiResult<LockSeatDto>.Failure("Ghế này đã được đặt");

            if (seat.Status == SeatStatus.Locked && seat.LockedBy != _currentUser.Id)
                return ApiResult<LockSeatDto>.Failure("Ghế này đang được giữ bởi người khác");

            if (bookingDetail.FlightSeatId.HasValue)
            {
                var oldSeat = await _unitOfWork.FlightSeatRepository
                    .GetByCondition(s => s.FlightSeatId == bookingDetail.FlightSeatId.Value)
                    .Include(s => s.SeatTemplate)
                    .FirstOrDefaultAsync(cancellationToken);

                if (oldSeat != null && oldSeat.Status == SeatStatus.Locked)
                {
                    oldSeat.Status = SeatStatus.Available;
                    oldSeat.LockedUntil = default;
                    oldSeat.LockedBy = 0;
                    _unitOfWork.FlightSeatRepository.Update(oldSeat);
                    await _hubContext.Clients
                        .Group(SeatHub.GetGroupName(bookingDetail.FlightId))
                        .SeatUnlocked(oldSeat.FlightSeatId, oldSeat.SeatTemplate.SeatNumber);
                }

                bookingDetail.FlightSeatId = null;
                _unitOfWork.BookingDetailRepository.Update(bookingDetail);
            }

            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == bookingDetail.BookingId)
                .FirstOrDefaultAsync(cancellationToken);

            if (seat.SeatTemplate.ClassId != booking.ClassId)
                return ApiResult<LockSeatDto>.Failure($"Ghế này không thuộc hạng {seat.SeatTemplate.SeatClass.ClassName}. " + $"Vui lòng chọn ghế đúng hạng của bạn");

            try
            {
                var now = DateTime.UtcNow;
                seat.Status = SeatStatus.Locked;
                seat.LockedUntil = now.AddMinutes(15);
                seat.LockedBy = _currentUser.Id!.Value;
                _unitOfWork.FlightSeatRepository.Update(seat);

                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ApiResult<LockSeatDto>.Failure("Ghế vừa được người khác chọn. Vui lòng thử ghế khác.");
            }

            await _hubContext.Clients
                .Group(SeatHub.GetGroupName(bookingDetail.FlightId))
                .SeatLocked(seat.FlightSeatId, seat.SeatTemplate.SeatNumber, _currentUser.Id!.Value);

            return ApiResult<LockSeatDto>.Success(new LockSeatDto
            {
                FlightSeatId = seat.FlightSeatId,
                SeatNumber = seat.SeatTemplate.SeatNumber,
                LockedUntil = seat.LockedUntil
            });
        }
    }
}
