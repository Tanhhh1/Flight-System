using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Planes.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Planes.Queries.GetAll
{
    public class GetAllPlaneQuery : IRequest<ApiResult<PageList<PlaneDto>>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Plane", 30);
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public FlightStatus? Status { get; set; }
    }
}
