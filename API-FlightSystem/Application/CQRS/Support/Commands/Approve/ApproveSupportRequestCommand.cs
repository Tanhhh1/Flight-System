using Application.Common;
using Application.CQRS.Support.DTOs;
using MediatR;

namespace Application.CQRS.Support.Commands.Approve
{
    public class ApproveSupportRequestCommand : IRequest<ApiResult<SupportRequestDto>>
    {
        public int RequestId { get; set; }
    }
}
