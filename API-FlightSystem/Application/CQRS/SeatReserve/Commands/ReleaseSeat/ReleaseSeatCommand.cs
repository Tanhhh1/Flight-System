using Application.Common;
using MediatR;

namespace Application.CQRS.SeatReserve.Commands.ReleaseSeat
{
    public class ReleaseSeatCommand : IRequest<ApiResult<bool>>
    {
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public int FlightSeatId { get; set; }
        public int PassengerId { get; set; }
    }
}
