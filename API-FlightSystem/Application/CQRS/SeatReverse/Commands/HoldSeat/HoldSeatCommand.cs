using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using MediatR;

namespace Application.CQRS.SeatReverse.Commands.HoldSeat
{
    public class HoldSeatCommand : IRequest<ApiResult<HoldSeatDto>>
    {
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public int PassengerId { get; set; }
    }
}
