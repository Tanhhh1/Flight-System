using Application.Common;
using Application.CQRS.Auth.DTOs;
using Application.Interfaces.Services;
using Application.Interfaces.UnitOfWork;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Identity;

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
            var jwtId = _tokenService.GetJwtId(request.AccessToken);
            if (jwtId == null)
                return ApiResult<SignInDto>.Failure(["Access token không hợp lệ"]);

            var refreshToken = await _unitOfWork.RefreshTokenRepository
                .GetByCondition(x => x.Token == request.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (refreshToken == null)
                return ApiResult<SignInDto>.Failure(["Refresh token không tồn tại"]);

            if (refreshToken.IsUsed)
                return ApiResult<SignInDto>.Failure(["Refresh token đã được sử dụng"]);

            if (refreshToken.InRevoked)
                return ApiResult<SignInDto>.Failure(["Refresh token đã bị thu hồi"]);

            if (refreshToken.ExpiryTime < DateTime.UtcNow)
                return ApiResult<SignInDto>.Failure(["Refresh token đã hết hạn"]);

            if (refreshToken.JwtId != jwtId)
                return ApiResult<SignInDto>.Failure(["Token không khớp"]);

            refreshToken.IsUsed = true;
            _unitOfWork.RefreshTokenRepository.Update(refreshToken);

            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null)
                return ApiResult<SignInDto>.Failure(["Người dùng không tồn tại"]);

            if (!user.IsActive)
                return ApiResult<SignInDto>.Failure(["Tài khoản đã bị vô hiệu hóa"]);

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

            await _unitOfWork.SaveChangesAsync();

            return ApiResult<SignInDto>.Success(new SignInDto
            {
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                AccessTokenExpires = tokenResult.AccessTokenExpires
            });
        }
    }
}
