using Application.Common;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Auth.Commands.SignUp
{
    public class SignUpHandler : IRequestHandler<SignUpCommand, ApiResult<string>>
    {
        private readonly UserManager<User> _userManager;
        public SignUpHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<string>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail is not null)
                return ApiResult<string>.Failure([new FieldError("Email", "Email đã được sử dụng")]);

            var existingUsername = await _userManager.FindByNameAsync(request.UserName);
            if (existingUsername is not null)
                return ApiResult<string>.Failure([new FieldError("Username", "Username đã được sử dụng")]);

            var user = request.Adapt<User>();

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<string>.Failure(errors);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addRoleResult.Succeeded)
            {
                var errors = addRoleResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<string>.Failure(errors);
            }

            return ApiResult<string>.Success("Đăng ký thành công.");
        }
    }
}