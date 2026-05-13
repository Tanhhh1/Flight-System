using Application.Common;
using Application.CQRS.Auth.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Identity;

namespace Application.CQRS.Auth.Commands.SignIn
{
    public class SignInHandler : IRequestHandler<SignInCommand, ApiResult<SignInDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public SignInHandler(UserManager<User> userManager, IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<SignInDto>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = request.LoginId.Contains('@')
                ? await _userManager.FindByEmailAsync(request.LoginId)
                : await _userManager.FindByNameAsync(request.LoginId);

            if (user == null)
                return ApiResult<SignInDto>.Failure(["Email, tên đăng nhập hoặc mật khẩu không chính xác"]);

            if (!user.IsActive)
                return ApiResult<SignInDto>.Failure(["Tài khoản đã bị vô hiệu hóa"]);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return ApiResult<SignInDto>.Failure(["Email, tên đăng nhập hoặc mật khẩu không chính xác"]);

            var tokenResult = await _tokenService.GenerateAsync(user);

            await _unitOfWork.RefreshTokenRepository.AddAsync(new Domain.Identity.RefreshToken
            {
                UserId = user.Id,
                JwtId = tokenResult.JwtId,
                Token = tokenResult.RefreshToken,
                ExpiryTime = tokenResult.RefreshTokenExpires,
                IsUsed = false,
                InRevoked = false,
                CreatedAt = DateTime.UtcNow,
            });

            return ApiResult<SignInDto>.Success(new SignInDto
            {
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                AccessTokenExpires = tokenResult.AccessTokenExpires
            });
        }
    }
}
