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
            if (user is null)
                return ApiResult<AccountDto>.Failure([new FieldError(null, "Tài khoản không tồn tại")]);

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingByEmail is not null && existingByEmail.Id != request.UserId)
                return ApiResult<AccountDto>.Failure([new FieldError("Email", "Email đã tồn tại trong hệ thống")]);

            request.Adapt(user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<AccountDto>.Failure(errors);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("user"))
                return ApiResult<AccountDto>.Failure([new FieldError(null, "Không thể chỉnh sửa tài khoản người dùng thông thường.")]);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = removeResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<AccountDto>.Failure(errors);
            }

            var addRoleResult = await _userManager.AddToRolesAsync(user, request.RoleNames);
            if (!addRoleResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, currentRoles);
                var errors = addRoleResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<AccountDto>.Failure(errors);
            }

            var accountDto = user.Adapt<AccountDto>();
            accountDto.Roles = request.RoleNames;

            return ApiResult<AccountDto>.Success(accountDto);
        }
    }
}