using Application.Common;
using Application.CQRS.Support.DTOs;
using MediatR;

namespace Application.CQRS.Support.Queries.GetById
{
    public class GetMySupportRequestsQuery : IRequest<ApiResult<PageList<SupportRequestDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
