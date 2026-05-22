using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Flights.DTOs;
using Application.Interfaces.CQRS;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Flights.Queries.Search
{
    public class DataSearchQuery : IRequest<ApiResult<DataSearchDto>>, ICacheable, IQuery
    {
        public HashSet<DataSearch> Include { get; set; } = [];

        public CacheProfile GetCacheProfile()
        {
            var key = string.Join("_", Include.OrderBy(x => x));
            return CacheProfile.Of($"DataSearch_{key}", 5);
        }
    }
}
