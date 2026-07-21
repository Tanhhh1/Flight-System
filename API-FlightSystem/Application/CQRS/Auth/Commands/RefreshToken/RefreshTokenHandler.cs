using Application.Common;
using Application.CQRS.Auth.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Auth.Commands.RefreshToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ApiResult<SignInDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;

        public RefreshTokenHandler(IUnitOfWork unitOfWork, ITokenService tokenService, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<ApiResult<SignInDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var hashedToken = _tokenService.HashToken(request.RefreshToken);

            var existingToken = await _unitOfWork.RefreshTokenRepository
                .GetByCondition(rt => rt.Token == hashedToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingToken is null)
                return ApiResult<SignInDto>.Failure("Refresh token không hợp lệ hoặc đã hết hạn");

            if (existingToken.IsRevoked)
                return ApiResult<SignInDto>.Failure("Refresh token không hợp lệ hoặc đã hết hạn");

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
                return ApiResult<SignInDto>.Failure("Refresh token không hợp lệ hoặc đã hết hạn");

            var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());
            if (user is null)
                return ApiResult<SignInDto>.Failure("Refresh token không hợp lệ hoặc đã hết hạn");

            existingToken.IsRevoked = true;
            _unitOfWork.RefreshTokenRepository.Update(existingToken);

            var signInResult = await _tokenService.GenerateAsync(user);

            var newRefreshToken = new Domain.Identity.RefreshToken
            {
                UserId = user.Id,
                Token = _tokenService.HashToken(signInResult.RefreshToken),
                ExpiresAt = signInResult.RefreshTokenExpires,
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken);
            return ApiResult<SignInDto>.Success(signInResult);
        }
    }
}
