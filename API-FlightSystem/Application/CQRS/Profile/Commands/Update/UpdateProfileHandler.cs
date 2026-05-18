using Application.Common;
using Application.CQRS.Profile.DTOs;
using Application.Interfaces.Services;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Profile.Commands.Update
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, ApiResult<UserProfileDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUser _currentUser;
        public UpdateProfileHandler(UserManager<User> userManager, ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
                return ApiResult<UserProfileDto>.Failure(["Người dùng chưa đăng nhập"]);

            var user = await _userManager.FindByIdAsync(_currentUser.Id.ToString()!);
            if (user is null)
                return ApiResult<UserProfileDto>.Failure(["Không tìm thấy người dùng"]);

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email!);
            if (existingByEmail != null && existingByEmail.Id != _currentUser.Id)
                return ApiResult<UserProfileDto>.Failure(["Email đã được sử dụng bởi tài khoản khác"]);

            request.Adapt(user);
            
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ApiResult<UserProfileDto>.Failure(result.Errors.Select(e => e.Description).ToList());

            var userDto = user.Adapt<UserProfileDto>();
            return ApiResult<UserProfileDto>.Success(userDto);
        }
    }
}
