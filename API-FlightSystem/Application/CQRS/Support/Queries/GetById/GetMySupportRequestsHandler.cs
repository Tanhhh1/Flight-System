using Application.Common;
using Application.CQRS.Support.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Support.Queries.GetById
{
    public class GetMySupportRequestsHandler : IRequestHandler<GetMySupportRequestsQuery, ApiResult<PageList<SupportRequestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public GetMySupportRequestsHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<PageList<SupportRequestDto>>> Handle(GetMySupportRequestsQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<PageList<SupportRequestDto>>.Failure("Bạn cần đăng nhập");

            var query = _unitOfWork.SupportRequestRepository
                .GetByCondition(sr => sr.Booking.UserId == _currentUser.Id)
                .OrderByDescending(sr => sr.CreatedAt);

            var pagedList = await PageList<SupportRequestDto>.ToPagedListAsync(
                query.AsNoTracking().ProjectToType<SupportRequestDto>(),
                request.PageIndex,
                request.PageSize,
                cancellationToken
            );

            return ApiResult<PageList<SupportRequestDto>>.Success(pagedList);
        }
    }
}
