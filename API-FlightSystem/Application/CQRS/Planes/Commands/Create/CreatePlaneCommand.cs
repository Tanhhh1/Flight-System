using Application.Common;
using Application.CQRS.Planes.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Planes.Commands.Create
{
    public class CreatePlaneCommand : IRequest<ApiResult<PlaneDto>>
    {
        public string PlaneModel { get; set; } = string.Empty;
        public int AirlineId { get; set; }
        public FlightStatus Status { get; set; }
    }
}
