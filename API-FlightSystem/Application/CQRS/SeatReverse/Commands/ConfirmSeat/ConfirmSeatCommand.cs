using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using MediatR;

namespace Application.CQRS.SeatReverse.Commands.ConfirmSeat
{
    public class ConfirmSeatCommand : IRequest<ApiResult<bool>>
    {
        public int BookingId { get; set; }
        public List<ConfirmSeatDto> Items { get; set; } = new();
    }
}
