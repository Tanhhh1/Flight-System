using Application.Interfaces.Repositories.Common;
using Domain.Identity;

namespace Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
    }
}
