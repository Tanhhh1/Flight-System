using Application.Common;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Planes.Commands.Delete
{
    public class DeletePlaneCommand : IRequest<ApiResult<PlaneDto>>, ICommand, IInvalidateCache
    {
        public IEnumerable<string> InvalidatePrefixes => ["Plane"];
        public int PlaneId { get; set; }
    }
}
