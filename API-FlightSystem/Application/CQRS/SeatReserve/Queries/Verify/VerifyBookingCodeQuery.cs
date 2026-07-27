using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using MediatR;

namespace Application.CQRS.SeatReserve.Queries.Verify
{
    public class VerifyBookingQuery : IRequest<ApiResult<VerifyBookingDto>>
    {
        public string BookingCode { get; set; } = string.Empty;
    }
}
