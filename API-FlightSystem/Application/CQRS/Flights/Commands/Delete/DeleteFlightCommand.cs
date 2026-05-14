using Application.Common;
using Application.CQRS.Flights.DTOs;
using MediatR;

namespace Application.CQRS.Flights.Commands.Delete
{
    public class DeleteFlightCommand : IRequest<ApiResult<FlightDto>>
    {
        public int FlightId { get; set; }
    }
}
