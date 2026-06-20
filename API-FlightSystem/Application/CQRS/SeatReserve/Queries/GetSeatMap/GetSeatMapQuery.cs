using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.SeatReserve.Queries.GetSeatMap
{
    public class GetSeatMapQuery : IQuery, IRequest<ApiResult<SeatMapDto>>
    {
        public int FlightId { get; set; }
        public int BookingId { get; set; }
    }
}
