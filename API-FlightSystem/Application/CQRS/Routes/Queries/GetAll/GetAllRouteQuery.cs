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
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? OriginAirportCode { get; set; }
        public string? DestinationAirportCode { get; set; }
        public FlightStatus? Status { get; set; }
    }
}
