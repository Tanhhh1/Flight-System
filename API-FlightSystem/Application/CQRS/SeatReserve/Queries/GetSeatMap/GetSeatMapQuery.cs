using Application.Common;
using Application.CQRS.SeatReserve.DTOs;
using MediatR;

namespace Application.CQRS.SeatReserve.Queries.GetSeatMap
{
    public class GetSeatMapQuery : IRequest<ApiResult<SeatMapDto>>
    {
        public int FlightId { get; set; }
        public int BookingId { get; set; }
    }
}
