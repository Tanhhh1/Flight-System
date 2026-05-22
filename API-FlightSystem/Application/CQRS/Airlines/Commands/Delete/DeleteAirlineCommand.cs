using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Delete
{
    public class DeleteAirlineCommand : IRequest<ApiResult<AirlineDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Airline"];
        public int AirlineId { get; set; }
    }
}
