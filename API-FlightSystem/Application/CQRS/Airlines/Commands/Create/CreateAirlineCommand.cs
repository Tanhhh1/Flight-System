using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Create
{
    public class CreateAirlineCommand : IRequest<ApiResult<AirlineDto>>
    { 
        public string AirlineName { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
    }
}
