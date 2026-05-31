namespace Shared.Identity
{
    public class JwtSetting
    {
        public string SecretKey { get; set; } = "";
        public string ValidAudience { get; set; } = "";
        public string ValidIssuer { get; set; } = "";
        public int TokenValidityInHours { get; set; } = 1;
        public int RefreshTokenValidityInDays { get; set; } = 7;
    }
}
