using Application.Common;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airports.Commands.Update
{
    public class UpdateAirportCommand : IRequest<ApiResult<AirportDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Airport"];
        public int AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
    }
}
