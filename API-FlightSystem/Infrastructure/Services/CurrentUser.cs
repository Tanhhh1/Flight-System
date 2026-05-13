using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private ClaimsPrincipal? User => _contextAccessor.HttpContext?.User;

        public CurrentUser(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public int? Id
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public string Email
            => User?.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;

        public string Username
            => User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? string.Empty;

        public string Fullname
            => User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? string.Empty;

        public List<string> Roles
            => User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? [];

        public bool IsAuthenticated
            => User?.Identity?.IsAuthenticated ?? false;

        public bool IsAdmin
            => Roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));

        public bool IsUser
            => Roles.Any(r => r.Equals("User", StringComparison.OrdinalIgnoreCase));

        public bool IsStaff
            => Roles.Any(r => r.Equals("Staff", StringComparison.OrdinalIgnoreCase));
    }
}
