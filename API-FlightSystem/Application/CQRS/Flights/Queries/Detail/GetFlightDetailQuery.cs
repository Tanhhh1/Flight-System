using Application.Common;
using Application.CQRS.Flights.DTOs;
using MediatR;

namespace Application.CQRS.Flights.Queries.Detail
{
    public class GetFlightDetailQuery : IRequest<ApiResult<FlightDetailDto>>
    {
        public int FlightId { get; set; }
    }
}
