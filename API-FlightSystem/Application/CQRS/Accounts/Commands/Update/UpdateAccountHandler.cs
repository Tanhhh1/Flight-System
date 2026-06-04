using Application.Common;
using Application.CQRS.Accounts.DTOs;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Accounts.Commands.Update
{
    public class UpdateAccountHandler : IRequestHandler<UpdateAccountCommand, ApiResult<AccountDto>>
    {
        private readonly UserManager<User> _userManager;
        public UpdateAccountHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<AccountDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return ApiResult<AccountDto>.Failure("Tài khoản không tồn tại");

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingByEmail != null && existingByEmail.Id != request.UserId)
                return ApiResult<AccountDto>.Failure([new FieldError("Email", "Email đã tồn tại trong hệ thống")]);

            request.Adapt(user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ApiResult<AccountDto>.Failure(string.Join(", ", updateResult.Errors.Select(e => e.Description)));

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("user"))
                return ApiResult<AccountDto>.Failure("Không thể chỉnh sửa tài khoản người dùng thông thường.");

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return ApiResult<AccountDto>.Failure(string.Join(", ", removeResult.Errors.Select(e => e.Description)));

            var addRoleResult = await _userManager.AddToRolesAsync(user, request.RoleNames);
            if (!addRoleResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, currentRoles);
                return ApiResult<AccountDto>.Failure(string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }

            var accountDto = user.Adapt<AccountDto>();
            accountDto.Roles = request.RoleNames;

            return ApiResult<AccountDto>.Success(accountDto);
        }
    }
}
