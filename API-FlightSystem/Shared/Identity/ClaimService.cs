using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Shared.Identity
{
    public class ClaimService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ClaimService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private HttpContext? Context
        {
            get
            {
                return _contextAccessor.HttpContext;
            }
        }

        public int UserId
        {
            get
            {
                var claimValue = Context?.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                                 ?? Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(claimValue, out var id))
                {
                    return id;
                }

                return 0;
            }
        }

        public string Email
        {
            get
            {
                return Context?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            }
        }

        public string Fullname
        {
            get
            {
                return Context?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            }
        }
        public List<string> Roles
        {
            get
            {
                var u = Context?.User;
                if (u != null)
                {
                    return u.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();
                }
                return new List<string>();
            }
        }

        public bool IsAdmin
        {
            get
            {
                return Roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool IsUser
        {
            get
            {
                return Roles.Any(r => r.Equals("User", StringComparison.OrdinalIgnoreCase));
            }
        }

        public bool IsStaff
        {
            get
            {
                return Roles.Any(r => r.Equals("Staff", StringComparison.OrdinalIgnoreCase));
            }
        }

        public string AccessToken
        {
            get
            {
                try
                {
                    return Context?.GetTokenAsync("access_token").GetAwaiter().GetResult() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}