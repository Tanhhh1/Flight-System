using Application.CQRS.Auth.DTOs;
using Domain.Identity;

namespace Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<SignInDto> GenerateAsync(User user);
        string? GetJwtId(string accessToken);
        string HashToken(string rawToken);
    }
}
