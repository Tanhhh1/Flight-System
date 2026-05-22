using Application.Common;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Routes.Commands.Update
{
    public class UpdateRouteCommand : IRequest<ApiResult<RouteDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Route"];
        public int RouteId { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public int FlightDuration { get; set; }
        public FlightStatus Status { get; set; }
    }
}
