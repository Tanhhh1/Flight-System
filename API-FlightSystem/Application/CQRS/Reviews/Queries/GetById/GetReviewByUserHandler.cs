using Application.Common;
using Application.CQRS.Bookings.DTOs;
using Application.CQRS.Bookings.Queries.Transaction;
using Application.CQRS.Reviews.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Reviews.Queries.GetById
{
    public class GetReviewByUserHandler : IRequestHandler<GetReviewByUserQuery, ApiResult<PageList<ReviewDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public GetReviewByUserHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<ApiResult<PageList<ReviewDto>>> Handle(GetReviewByUserQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<PageList<ReviewDto>>.Failure("Bạn cần đăng nhập để xem lịch sử đánh giá");

            var review = _unitOfWork.ReviewRepository
                .GetByCondition(b => b.UserId == _currentUser.Id)
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking();

            var pagedList = await PageList<ReviewDto>.ToPagedListAsync(
                review.ProjectToType<ReviewDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<ReviewDto>>.Success(pagedList);
        }
    }
}
