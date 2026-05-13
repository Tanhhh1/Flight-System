using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class Role : IdentityRole<int>
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
