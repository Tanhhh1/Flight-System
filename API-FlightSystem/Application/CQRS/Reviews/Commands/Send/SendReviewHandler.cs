using Application.Common;
using Application.CQRS.Reviews.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.CQRS.Reviews.Commands.Send
{
    public class SendReviewHandler : IRequestHandler<SendReviewCommand, ApiResult<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        public SendReviewHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<ApiResult<ReviewDto>> Handle(SendReviewCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
                return ApiResult<ReviewDto>.Failure(["Bạn cần đăng nhập để thực hiện chức năng này."]);

            var review = request.Adapt<Review>();
            review.UserId = _currentUser.Id.Value;
            review.IsHidden = false;

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            var reviewDto = review.Adapt<ReviewDto>();
            return ApiResult<ReviewDto>.Success(reviewDto);
        }
    }
}
