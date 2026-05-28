using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Update
{
    public class UpdateAirlineCommand : IRequest<ApiResult<AirlineDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Airline"];
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public string AirlineCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }

    }
}
