using Application.Common;
using Application.CQRS.Accounts.DTOs;
using MediatR;

namespace Application.CQRS.Accounts.Commands.Update
{
    public class UpdateAccountCommand : IRequest<ApiResult<AccountDto>>
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public IEnumerable<string> RoleNames { get; set; } = new List<string>();
    }
}
