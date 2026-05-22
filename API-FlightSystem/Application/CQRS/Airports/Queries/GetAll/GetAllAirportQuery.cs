using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetAll
{
    public class GetAllAirportQuery : IRequest<ApiResult<PageList<AirportDto>>>, ICacheable, IQuery
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public FlightStatus? Status { get; set; }
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Airport", 30);
    }
}
