using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using MediatR;

namespace Application.CQRS.SeatReverse.Commands.LockSeat
{
    public class LockSeatCommand : IRequest<ApiResult<LockSeatDto>>
    {
        public int FlightSeatId { get; set; }
        public int BookingDetailId { get; set; }
    }
}
