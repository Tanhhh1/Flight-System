using Domain.Common;
using Domain.Identity;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public User User { get; set; } = null!;
    }
}
