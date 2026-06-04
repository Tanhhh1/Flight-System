using Application.Common;
using Application.CQRS.SeatReverse.DTOs;
using MediatR;

namespace Application.CQRS.SeatReverse.Queries.GetFlightSeat
{
    public class GetFlightSeatQuery : IRequest<ApiResult<List<SeatLayoutDto>>>
    {
        public int FlightId { get; set; }
    }
}
