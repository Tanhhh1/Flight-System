using Application.Common;
using Domain.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Auth.Commands.SignUp
{
    public class SignUpHandler : IRequestHandler<SignUpCommand, ApiResult<bool>>
    {
        private const string DefaultRole = "User";
        private readonly UserManager<User> _userManager;
        public SignUpHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApiResult<bool>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var existingByUsername = await _userManager.FindByNameAsync(request.UserName);
            if (existingByUsername is not null)
                return ApiResult<bool>.Failure([new FieldError("Username", "Tên đăng nhập đã tồn tại")]);

            var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingByEmail is not null)
                return ApiResult<bool>.Failure([new FieldError("Email", "Email đã được sử dụng")]);

            var user = request.Adapt<User>();

            var signUpResult = await _userManager.CreateAsync(user, request.Password);
            if (!signUpResult.Succeeded)
            {
                var errors = signUpResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<bool>.Failure(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, DefaultRole);
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors.Select(e => new FieldError(null, e.Description));
                return ApiResult<bool>.Failure(errors);
            }

            return ApiResult<bool>.Success(true);
        }
    }
}