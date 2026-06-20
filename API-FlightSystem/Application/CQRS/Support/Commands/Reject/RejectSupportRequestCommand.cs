using Application.Common;
using Application.CQRS.Support.DTOs;
using MediatR;

namespace Application.CQRS.Support.Commands.Reject
{
    public class RejectSupportRequestCommand : IRequest<ApiResult<SupportRequestDto>>
    {
        public int RequestId { get; set; }
    }
}
