using Application.Common;
using Application.CQRS.Auth.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CQRS.Auth.Commands.SignIn
{
    public class SignInHandler : IRequestHandler<SignInCommand, ApiResult<SignInDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public SignInHandler(UserManager<User> userManager, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<SignInDto>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                return ApiResult<SignInDto>.Failure("Tên đăng nhập hoặc mật khẩu không chính xác");

            if (!user.IsActive)
                return ApiResult<SignInDto>.Failure("Tài khoản của bạn đã bị khóa");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return ApiResult<SignInDto>.Failure("Tên đăng nhập hoặc mật khẩu không chính xác");

            var signInResult = await _tokenService.GenerateAsync(user);

            var refreshToken = new Domain.Identity.RefreshToken
            {
                UserId = user.Id,
                Token = _tokenService.HashToken(signInResult.RefreshToken),
                ExpiresAt = signInResult.RefreshTokenExpires,
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            return ApiResult<SignInDto>.Success(signInResult);
        }
    }
}
