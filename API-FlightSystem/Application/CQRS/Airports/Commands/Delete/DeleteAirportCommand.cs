using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Airports.Commands.Delete
{
    public class DeleteAirportCommand : IRequest<ApiResult<AirportDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Airport"];
        public int AirportId { get; set; }
    }
}
