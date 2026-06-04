using Application.Common;
using MediatR;

namespace Application.CQRS.SeatReverse.Commands.UnlockSeat
{
    public class UnlockSeatCommand : IRequest<ApiResult<bool>>
    {
        public int FlightSeatId { get; set; }
        public int BookingDetailId { get; set; }
    }
}
