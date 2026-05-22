using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Airports.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Airports.Queries.GetById
{
    public class GetByAirportIdQuery : IRequest<ApiResult<AirportDto>>, ICacheable, IQuery
    {
        public int AirportId { get; set; }
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Airport", 30);
    }
}
