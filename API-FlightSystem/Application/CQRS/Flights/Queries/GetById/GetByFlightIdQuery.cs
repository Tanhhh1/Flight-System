using Application.Common;
using Application.CQRS.Flights.DTOs;
using MediatR;

namespace Application.CQRS.Flights.Queries.GetById
{
    public class GetByFlightIdQuery : IRequest<ApiResult<FlightDto>>
    {
        public int FlightId { get; set; }
    }
}
