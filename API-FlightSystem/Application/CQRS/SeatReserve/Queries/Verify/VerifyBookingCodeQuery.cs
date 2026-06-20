using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.SeatReserve.Queries.Verify
{
    public class VerifyBookingQuery : IQuery, IRequest<ApiResult<VerifyBookingDto>>
    {
        public string BookingCode { get; set; } = string.Empty;
    }
}
