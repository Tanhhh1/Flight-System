using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Services.Queries.GetById
{
    public class GetByServiceIdQuery : IRequest<ApiResult<ServiceDto>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Service", 30);
        public int ServiceId { get; set; }
    }
}
