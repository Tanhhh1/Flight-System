using Application.Common;
using Application.CQRS.Services.DTOs;
using MediatR;

namespace Application.CQRS.Services.Queries.GetAll
{
    public class GetAllServiceQuery : IRequest<ApiResult<PageList<ServiceDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
