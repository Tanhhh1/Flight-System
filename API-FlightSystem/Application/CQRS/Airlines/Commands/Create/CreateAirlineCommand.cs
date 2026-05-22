using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Create
{
    public class CreateAirlineCommand : IRequest<ApiResult<AirlineDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Airline"];
        public string AirlineName { get; set; } = string.Empty;
    }
}
