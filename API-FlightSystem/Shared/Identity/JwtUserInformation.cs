namespace Shared.Identity
{
    public class JwtUserInformation
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
