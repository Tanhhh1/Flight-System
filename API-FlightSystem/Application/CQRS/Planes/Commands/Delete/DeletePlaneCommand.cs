using Application.Common;
using Application.CQRS.Planes.DTOs;
using MediatR;

namespace Application.CQRS.Planes.Commands.Delete
{
    public class DeletePlaneCommand : IRequest<ApiResult<PlaneDto>>
    {
        public int PlaneId { get; set; }
    }
}
