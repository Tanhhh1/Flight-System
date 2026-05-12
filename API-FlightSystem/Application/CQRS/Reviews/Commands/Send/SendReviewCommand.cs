using Application.Common;
using Application.CQRS.Reviews.DTOs;
using MediatR;

namespace Application.CQRS.Reviews.Commands.Send
{
    public class SendReviewCommand : IRequest<ApiResult<ReviewDto>>
    {
        public string Content { get; set; } = string.Empty;
        public bool IsHidden { get; set; } = false;
    }
}
