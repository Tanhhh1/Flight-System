using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Domain.Identity
{
    public class User : IdentityUser<int>
    {
        public string Fullname { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
