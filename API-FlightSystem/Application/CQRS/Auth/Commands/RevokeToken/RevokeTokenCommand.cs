using Application.Common;
using MediatR;

namespace Application.CQRS.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommand : IRequest<ApiResult<string>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
