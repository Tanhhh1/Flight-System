using Application.Common;
using Application.CQRS.Reviews.DTOs;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;

namespace Application.CQRS.Reviews.Commands.Delete
{
    public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, ApiResult<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<ReviewDto>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(request.ReviewId);
            if (review == null)
                return ApiResult<ReviewDto>.Failure("Đánh giá không tồn tại");

            review.IsHidden = true; 
            var reviewDto = review.Adapt<ReviewDto>();
            return ApiResult<ReviewDto>.Success(reviewDto);
        }
    }
}
