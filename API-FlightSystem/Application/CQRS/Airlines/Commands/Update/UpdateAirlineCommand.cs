using Application.Common;
using Application.CQRS.Airlines.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airlines.Commands.Update
{
    public class UpdateAirlineCommand : IRequest<ApiResult<AirlineDto>>
    {
        public int AirlineId { get; set; }
        public string AirlineName { get; set; } = string.Empty;
        public FlightStatus Status { get; set; }
    }
}
