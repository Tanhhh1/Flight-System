using Application.Common;
using Application.CQRS.Routes.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Routes.Commands.Update
{
    public class UpdateRouteCommand : IRequest<ApiResult<RouteDto>>
    {
        public int RouteId { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public int FlightDuration { get; set; }
        public FlightStatus Status { get; set; }
    }
}
