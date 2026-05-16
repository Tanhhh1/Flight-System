using Application.Common;
using Application.CQRS.Auth.DTOs;
using MediatR;

namespace Application.CQRS.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<ApiResult<RefreshTokenDto>>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
