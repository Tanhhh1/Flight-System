using Application.Common;
using Application.Hubs;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace Application.CQRS.SeatReverse.Commands.ConfirmSeat
{
    public class ConfirmSeatsHandler : IRequestHandler<ConfirmSeatCommand, ApiResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IHubContext<SeatHub, ISeatHubClient> _hubContext;

        public ConfirmSeatsHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IHubContext<SeatHub, ISeatHubClient> hubContext)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _hubContext = hubContext;
        }

        public async Task<ApiResult<bool>> Handle(ConfirmSeatCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.BookingRepository
                .GetByCondition(b => b.BookingId == request.BookingId && b.UserId == _currentUser.Id)
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(cancellationToken);

            if (booking is null)
                return ApiResult<bool>.Failure("Không tìm thấy đơn đặt vé.");

            var flightId = booking.BookingDetails.First().FlightId;
            var seatIds = request.Items.Select(i => i.FlightSeatId).ToList();

            var seats = await _unitOfWork.FlightSeatRepository
                .GetByCondition(s => seatIds.Contains(s.FlightSeatId) && s.Status == SeatStatus.Locked && s.LockedBy == _currentUser.Id)
                .Include(s => s.SeatTemplate)
                .ToListAsync(cancellationToken);

            if (seats.Count != request.Items.Count)
                return ApiResult<bool>.Failure("Một số ghế đã hết hạn giữ. Vui lòng chọn lại.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var item in request.Items)
                {
                    var seat = seats.First(s => s.FlightSeatId == item.FlightSeatId);
                    var bookingDetail = booking.BookingDetails.First(bd => bd.BookingDetailId == item.BookingDetailId);
                    seat.Status = SeatStatus.Booked;
                    _unitOfWork.FlightSeatRepository.Update(seat);

                    bookingDetail.FlightSeatId = seat.FlightSeatId;
                    _unitOfWork.BookingDetailRepository.Update(bookingDetail);

                    await _hubContext.Clients.Group(SeatHub.GetGroupName(flightId)).SeatBooked(seat.FlightSeatId, seat.SeatTemplate.SeatNumber);
                }
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResult<bool>.Failure("Có lỗi xảy ra khi xác nhận ghế");
            }
            return ApiResult<bool>.Success(true);
        }
    }
}
