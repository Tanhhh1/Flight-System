using Application.Common;
using Application.Common.Caching;
using Application.CQRS.Services.DTOs;
using Application.Interfaces.CQRS;
using MediatR;

namespace Application.CQRS.Services.Queries.GetAll
{
    public class GetAllServiceQuery : IRequest<ApiResult<PageList<ServiceDto>>>, ICacheable, IQuery
    {
        public CacheProfile GetCacheProfile() => CacheProfile.Of("Service", 30);
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
