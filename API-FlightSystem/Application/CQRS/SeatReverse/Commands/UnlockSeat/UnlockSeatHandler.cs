using Application.Common;
using Application.Hubs;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.SeatReverse.Commands.UnlockSeat
{
    public class UnlockSeatHandler : IRequestHandler<UnlockSeatCommand, ApiResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IHubContext<SeatHub, ISeatHubClient> _hubContext;
        public UnlockSeatHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IHubContext<SeatHub, ISeatHubClient> hubContext)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _hubContext = hubContext;
        }

        public async Task<ApiResult<bool>> Handle(UnlockSeatCommand request, CancellationToken cancellationToken)
        {
            var bookingDetail = await _unitOfWork.BookingDetailRepository
                .GetByCondition(bd => bd.BookingDetailId == request.BookingDetailId && bd.Booking.UserId == _currentUser.Id)
                .Include(bd => bd.Booking)
                .FirstOrDefaultAsync(cancellationToken);

            if (bookingDetail == null)
                return ApiResult<bool>.Failure("Không tìm thấy thông tin đặt vé");

            var seat = await _unitOfWork.FlightSeatRepository
                .GetByCondition(s => s.FlightSeatId == request.FlightSeatId && s.LockedBy == _currentUser.Id && s.Status == SeatStatus.Locked)
                .Include(s => s.SeatTemplate)
                .FirstOrDefaultAsync(cancellationToken);

            if (seat == null)
                return ApiResult<bool>.Failure("Ghế không tồn tại hoặc không thuộc quyền của bạn.");

            seat.Status = SeatStatus.Available;
            seat.LockedUntil = default;
            seat.LockedBy = 0;
            _unitOfWork.FlightSeatRepository.Update(seat);

            bookingDetail.FlightSeatId = null;
            _unitOfWork.BookingDetailRepository.Update(bookingDetail);

            await _hubContext.Clients
                .Group(SeatHub.GetGroupName(bookingDetail.FlightId))
                .SeatUnlocked(seat.FlightSeatId, seat.SeatTemplate.SeatNumber);

            return ApiResult<bool>.Success(true);
        }
    }
}
