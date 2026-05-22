using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airlines.Queries.GetAll
{
    public class GetAllAirlineQuery : IRequest<ApiResult<PageList<AirlineDto>>>, ICacheable, IQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public FlightStatus? Status { get; set; }

        public CacheProfile GetCacheProfile() => CacheProfile.Of("Airline", 30);
    }
}
