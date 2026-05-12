using Application.Common;
using Application.CQRS.Accounts.DTOs;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Accounts.Commands.Delete
{
    public class DeleteAccountHandler : IRequestHandler<DeleteAccountCommand, ApiResult<AccountDto>>
    {
        private readonly UserManager<User> _userManager;
        public DeleteAccountHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<AccountDto>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return ApiResult<AccountDto>.Failure(["Tài khoản không tồn tại"]);

            if (!user.IsActive)
                return ApiResult<AccountDto>.Failure(["Tài khoản đã bị khóa trước đó"]);

            user.IsActive = false;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApiResult<AccountDto>.Failure(updateResult.Errors.Select(e => e.Description));

            var accountDto = user.Adapt<AccountDto>();
            return ApiResult<AccountDto>.Success(accountDto);
        }

        /* Kiểm tra quyền hạn của tài khoản
         * Không thể khóa chính tài khoản đang sử dụng
         * Thu hồi Refresh token
         * Hủy các đăng nhập và thông báo */
    }
}
