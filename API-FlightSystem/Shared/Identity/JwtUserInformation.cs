namespace Shared.Identity
{
    public class JwtUserInformation
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
