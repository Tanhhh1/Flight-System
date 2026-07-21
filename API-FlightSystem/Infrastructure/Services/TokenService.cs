using Application.CQRS.Auth.DTOs;
using Application.Interfaces.Services;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Helpers;
using Shared.Identity;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSetting _jwtSetting;
        private readonly UserManager<User> _userManager;

        public TokenService(IOptions<JwtSetting> jwtSetting, UserManager<User> userManager)
        {
            _jwtSetting = jwtSetting.Value;
            _userManager = userManager;
        }

        public string? GetJwtId(string accessToken)
            => JwtHelper.GetJwtIdIgnoreExpiry(_jwtSetting, accessToken);

        public string HashToken(string rawToken)
        {
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<SignInDto> GenerateAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var jwtUser = new JwtUserInformation
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Username = user.UserName ?? string.Empty
            };

            var issuedAt = DateTime.UtcNow;

            var accessToken = JwtHelper.GenerateToken(roles, _jwtSetting, jwtUser, issuedAt)
                ?? throw new InvalidOperationException("Không thể tạo access token. Vui lòng kiểm tra cấu hình JWT (SecretKey)");

            var jwtId = JwtHelper.GetJwtIdIgnoreExpiry(_jwtSetting, accessToken)
                ?? throw new InvalidOperationException("Không thể trích xuất JwtId từ access token vừa tạo");

            var rawRefreshToken = StringHelper.GenerateRefreshToken();

            return new SignInDto
            {
                JwtId = jwtId,
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                AccessTokenExpires = DateTime.SpecifyKind(issuedAt.AddHours(_jwtSetting.TokenValidityInHours), DateTimeKind.Utc),
                RefreshTokenExpires = DateTime.SpecifyKind(issuedAt.AddDays(_jwtSetting.RefreshTokenValidityInDays), DateTimeKind.Utc)
            };
        }
    }
}
