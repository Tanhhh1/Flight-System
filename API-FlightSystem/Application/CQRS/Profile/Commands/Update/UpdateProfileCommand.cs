using Application.Common;
using Application.CQRS.Profile.DTOs;
using MediatR;

namespace Application.CQRS.Profile.Commands.Update
{
    public class UpdateProfileCommand : IRequest<ApiResult<UserProfileDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
