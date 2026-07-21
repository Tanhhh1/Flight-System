using Microsoft.IdentityModel.Tokens;
using Shared.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shared.Helpers
{
    public class JwtHelper
    {
        public static string? GenerateToken(IEnumerable<string> roles, JwtSetting jwtSetting, JwtUserInformation user, DateTime expireStart)
        {
            if (string.IsNullOrEmpty(jwtSetting.SecretKey)) return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)), SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Name, user.Fullname),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r ?? "")));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtSetting.ValidIssuer,
                Audience = jwtSetting.ValidAudience,
                Subject = new ClaimsIdentity(claims),
                Expires = expireStart.AddHours(jwtSetting.TokenValidityInHours),
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string? VerifyToken(JwtSetting jwtSetting, string? token)
            => ValidateToken(jwtSetting, token, validateLifetime: true);

        public static string? GetJwtIdIgnoreExpiry(JwtSetting jwtSetting, string? token)
            => ValidateToken(jwtSetting, token, validateLifetime: false);
        private static string? ValidateToken(
            JwtSetting jwtSetting, string? token, bool validateLifetime)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token)) return null;

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSetting.ValidIssuer,
                ValidAudience = jwtSetting.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = !string.IsNullOrEmpty(jwtSetting.ValidIssuer),
                ValidateAudience = !string.IsNullOrEmpty(jwtSetting.ValidAudience),
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token, validationParameters, out _);

                return principal?.Claims
                    .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
    }
}
