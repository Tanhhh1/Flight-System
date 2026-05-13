using Application.CQRS.Auth.DTOs;
using Application.Interfaces.Services;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Helpers;
using Shared.Identity;


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

        public async Task<SignInDto> GenerateAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var jwtUser = new JwtUserInformation
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Username = user.UserName,
                Email = user.Email
            };

            var issuedAt = DateTime.UtcNow;

            var accessToken = JwtHelper.GenerateToken(
                roles,
                _jwtSetting,
                jwtUser,
                issuedAt
            );

            var jwtId = JwtHelper.GetJwtIdIgnoreExpiry(_jwtSetting, accessToken);

            return new SignInDto
            {
                JwtId = jwtId ?? string.Empty,
                AccessToken = accessToken!,
                RefreshToken = StringHelper.GenerateRefreshToken(),
                AccessTokenExpires = issuedAt.AddMinutes(_jwtSetting.TokenValidityInMinutes),
                RefreshTokenExpires = issuedAt.AddDays(_jwtSetting.RefreshTokenValidityInDays)
            };
        }
    }
}
