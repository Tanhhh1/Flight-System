using Application.Common;
using Application.Interfaces.Services;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace Application.CQRS.Profile.Commands.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ApiResult<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUser _currentUser;

        public ChangePasswordHandler(UserManager<User> userManager, ICurrentUser currentUser)
        {
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<ApiResult<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id is null)
                return ApiResult<string>.Failure("Người dùng chưa đăng nhập");

            var user = await _userManager.FindByIdAsync(_currentUser.Id.ToString()!);
            if (user is null)
                return ApiResult<string>.Failure("Không tìm thấy người dùng");

            var isCurrentPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isCurrentPassword)
                return ApiResult<string>.Failure("Mật khẩu hiện tại không đúng");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return ApiResult<string>.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.UpdateSecurityStampAsync(user);

            return ApiResult<string>.Success("Đổi mật khẩu thành công, vui lòng đăng nhập lại");
        }
    }
}
