using Application.Common;
using Application.CQRS.Support.DTOs;
using MediatR;

namespace Application.CQRS.Support.Queries.GetById
{
    public class GetSupportRequestByIdQuery : IRequest<ApiResult<SupportRequestDetailDto>>
    {
        public int RequestId { get; set; }
    }
}
