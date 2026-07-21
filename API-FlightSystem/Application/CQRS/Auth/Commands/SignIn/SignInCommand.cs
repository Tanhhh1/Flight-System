using Application.Common;
using Application.CQRS.Auth.DTOs;
using MediatR;

namespace Application.CQRS.Auth.Commands.SignIn
{
    public class SignInCommand : IRequest<ApiResult<SignInDto>>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
