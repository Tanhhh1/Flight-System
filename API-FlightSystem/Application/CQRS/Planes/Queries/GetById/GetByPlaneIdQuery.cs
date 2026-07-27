using Application.Common;
using Application.CQRS.Planes.DTOs;
using MediatR;

namespace Application.CQRS.Planes.Queries.GetById
{
    public class GetByPlaneIdQuery : IRequest<ApiResult<PlaneDto>>
    {
        public int PlaneId { get; set; }
    }
}
