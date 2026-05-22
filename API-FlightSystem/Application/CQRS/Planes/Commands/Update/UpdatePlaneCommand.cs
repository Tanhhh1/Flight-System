using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Planes.Commands.Update
{
    public class UpdatePlaneCommand : IRequest<ApiResult<PlaneDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Plane"];
        public int PlaneId { get; set; }
        public string PlaneModel { get; set; } = string.Empty;
        public int AirlineId { get; set; }
        public FlightStatus Status { get; set; }
    }
}
