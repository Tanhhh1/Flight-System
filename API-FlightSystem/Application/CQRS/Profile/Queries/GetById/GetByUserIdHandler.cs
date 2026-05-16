using Application.Common;
using Application.CQRS.Profile.DTOs;
using Application.Interfaces.Services;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Profile.Queries.GetById
{
    public class GetByUserIdHandler : IRequestHandler<GetByUserIdQuery, ApiResult<UserProfileDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUser _currentUser;

        public GetByUserIdHandler(UserManager<User> userManager, ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<UserProfileDto>> Handle(GetByUserIdQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<UserProfileDto>.Failure(["Người dùng chưa đăng nhập"]);

            var user = await _userManager.FindByIdAsync(_currentUser.Id.ToString()!);
            if (user is null)
                return ApiResult<UserProfileDto>.Failure(["Không tìm thấy người dùng"]);

            var dto = user.Adapt<UserProfileDto>();
            return ApiResult<UserProfileDto>.Success(dto);
        }
    }
}
