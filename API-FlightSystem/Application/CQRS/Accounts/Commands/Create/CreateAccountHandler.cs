using Application.Common;
using Application.CQRS.Accounts.DTOs;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Accounts.Commands.Create
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, ApiResult<AccountDto>>
    {
        private readonly UserManager<User> _userManager;
        public CreateAccountHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<AccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var existingByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (existingByUsername is not null)
                return ApiResult<AccountDto>.Failure([new FieldError("UserName", "Tên đăng nhập đã tồn tại trong hệ thống")]);

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingByEmail is not null)
                return ApiResult<AccountDto>.Failure([new FieldError("Email", "Email đã tồn tại trong hệ thống")]);

            var user = request.Adapt<User>();

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<AccountDto>.Failure(errors);
            }

            var addRoleResult = await _userManager.AddToRolesAsync(user, request.RoleNames);
            if (!addRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var errors = addRoleResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<AccountDto>.Failure(errors);
            }

            var accountDto = user.Adapt<AccountDto>();
            accountDto.IsActive = true;
            accountDto.Roles = request.RoleNames;

            return ApiResult<AccountDto>.Success(accountDto);
        }
    }
}