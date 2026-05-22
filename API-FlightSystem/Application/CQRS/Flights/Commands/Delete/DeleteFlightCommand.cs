using Application.Common;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Flights.Commands.Delete
{
    public class DeleteFlightCommand : IRequest<ApiResult<FlightDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Flight"];
        public int FlightId { get; set; }
    }
}
