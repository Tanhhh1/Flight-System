using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using MediatR;

namespace Application.CQRS.SeatReserve.Commands.ConfirmSeat
{
    public class ConfirmSeatsCommand : IRequest<ApiResult<bool>>
    {
        public int BookingId { get; set; }
        public Dictionary<int, List<SeatAssignmentDto>> Assignments { get; set; } = new();
    }
}
