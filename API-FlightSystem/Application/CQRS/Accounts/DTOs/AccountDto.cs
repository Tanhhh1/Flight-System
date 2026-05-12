namespace Application.CQRS.Accounts.DTOs
{
    public class AccountDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
