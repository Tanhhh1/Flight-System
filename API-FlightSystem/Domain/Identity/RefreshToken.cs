using Domain.Common;

namespace Domain.Identity
{
    public class RefreshToken : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public User User { get; set; } = null!;
    }
}
