using Application.Common;
using MediatR;

namespace Application.CQRS.Profile.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<ApiResult<string>>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
