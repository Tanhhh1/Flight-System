using Application.Common;
using Application.CQRS.Reviews.DTOs;
using MediatR;

namespace Application.CQRS.Reviews.Commands.Delete
{
    public class DeleteReviewCommand : IRequest<ApiResult<ReviewDto>>
    {
        public int ReviewId { get; set; }
    }
}
