using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.SeatReverse.Queries.Verify
{
    public class VerifyBookingQuery : IQuery, IRequest<ApiResult<VerifyBookingDto>>
    {
        public string BookingCode { get; set; } = string.Empty;
    }
}
