using Application.Common;
using Application.CQRS.Reviews.DTOs;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.CQRS.Reviews.Commands.Send
{
    public class SendReviewHandler : IRequestHandler<SendReviewCommand, ApiResult<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SendReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResult<ReviewDto>> Handle(SendReviewCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = 1;

            var review = request.Adapt<Review>();
            review.UserId = currentUserId;

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            var reviewDto = review.Adapt<ReviewDto>();
            return ApiResult<ReviewDto>.Success(reviewDto);
        }

        /* Thêm UserId của người dùng sau khi đăng nhập */
    }
}
