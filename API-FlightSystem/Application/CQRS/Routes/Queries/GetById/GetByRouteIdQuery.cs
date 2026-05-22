using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Routes.Queries.GetById
{
    public class GetByRouteIdQuery : IRequest<ApiResult<RouteDto>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Route", 30);
        public int RouteId { get; set; }
    }
}