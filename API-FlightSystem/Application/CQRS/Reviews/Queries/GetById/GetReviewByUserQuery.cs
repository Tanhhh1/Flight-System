using Application.Common;
using Application.CQRS.Reviews.DTOs;
using MediatR;

namespace Application.CQRS.Reviews.Queries.GetById
{
    public class GetReviewByUserQuery : IRequest<ApiResult<PageList<ReviewDto>>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
