using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using MediatR;

namespace Application.CQRS.SeatReserve.Commands.HoldSeat
{
    public class HoldSeatCommand : IRequest<ApiResult<HoldSeatDto>>
    {
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public int PassengerId { get; set; }
    }
}