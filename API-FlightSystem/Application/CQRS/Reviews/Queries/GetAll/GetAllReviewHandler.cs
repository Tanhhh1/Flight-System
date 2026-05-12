using Application.Common;
using Application.CQRS.Reviews.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Reviews.Queries.GetAll
{
    public class GetAllReviewHandler : IRequestHandler<GetAllReviewQuery, ApiResult<PageList<ReviewDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<PageList<ReviewDto>>> Handle(GetAllReviewQuery request, CancellationToken cancellationToken)
        {
            var review = _unitOfWork.ReviewRepository.GetByCondition();
            if (!string.IsNullOrEmpty(request.Search))
                review = review.Where(p => p.Content.Contains(request.Search));

            if (request.IsHidden.HasValue)
                review = review.Where(a => a.IsHidden == request.IsHidden.Value);

            review = review.OrderBy(a => a.ReviewId);

            var pagedList = await PageList<ReviewDto>.ToPagedListAsync(
                review.ProjectToType<ReviewDto>(),
                request.PageIndex,
                request.PageSize
            );
            return ApiResult<PageList<ReviewDto>>.Success(pagedList);
        }
    }
}
