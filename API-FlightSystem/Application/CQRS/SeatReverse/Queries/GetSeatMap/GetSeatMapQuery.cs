using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.SeatReverse.Queries.GetSeatMap
{
    public class GetSeatMapQuery : IQuery, IRequest<ApiResult<SeatMapDto>>
    {
        public int FlightId { get; set; }
        public int BookingId { get; set; }
    }
}
