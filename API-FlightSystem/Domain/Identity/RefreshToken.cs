using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Identity
{
    public class RefreshToken : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string JwtId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool InRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
