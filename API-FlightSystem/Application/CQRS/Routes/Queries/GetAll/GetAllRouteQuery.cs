using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Routes.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Routes.Queries.GetAll
{
    public class GetAllRouteQuery : IRequest<ApiResult<PageList<RouteDto>>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Route", 30);
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? OriginCity { get; set; }
        public string? DestinationCity { get; set; }
        public FlightStatus? Status { get; set; }
    }
}
