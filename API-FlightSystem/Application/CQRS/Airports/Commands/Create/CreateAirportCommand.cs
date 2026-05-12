using Application.Common;
using Application.CQRS.Airports.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airports.Commands.Create
{
    public class CreateAirportCommand : IRequest<ApiResult<AirportDto>>
    {
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public FlightStatus Status { get; set; } = FlightStatus.Active;
    }
}
