using Application.Common;
using Application.CQRS.Airports.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airports.Commands.Update
{
    public class UpdateAirportCommand : IRequest<ApiResult<AirportDto>>
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
    }
}
