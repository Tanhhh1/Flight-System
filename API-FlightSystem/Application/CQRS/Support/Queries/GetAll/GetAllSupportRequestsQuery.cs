using Application.Common;
using Application.CQRS.Support.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.CQRS.Support.Queries.GetAll
{
    public class GetAllSupportRequestsQuery : IRequest<ApiResult<PageList<SupportRequestDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public RequestType? RequestType { get; set; }
        public SupportStatus? Status { get; set; }
    }
}
