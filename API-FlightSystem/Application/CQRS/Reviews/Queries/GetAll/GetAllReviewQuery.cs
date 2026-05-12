using Application.Common;
using Application.CQRS.Reviews.DTOs;
using MediatR;

namespace Application.CQRS.Reviews.Queries.GetAll
{
    public class GetAllReviewQuery : IRequest<ApiResult<PageList<ReviewDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool? IsHidden { get; set; }
    }
}
