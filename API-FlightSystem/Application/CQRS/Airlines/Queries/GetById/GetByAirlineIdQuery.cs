using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Airlines.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Airlines.Queries.GetById
{
    public class GetByAirlineIdQuery : IRequest<ApiResult<AirlineDto>>, ICacheable, IQuery
    {
        public int AirlineId { get; set; }
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Airline", 30);
    }
}
