using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Planes.Queries.GetById
{
    public class GetByPlaneIdQuery : IRequest<ApiResult<PlaneDto>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Plane", 30);
        public int PlaneId { get; set; }
    }
}
