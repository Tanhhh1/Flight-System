using Application.Interfaces.Repositories;
using Domain.Identity;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository(DatabaseContext dbContext) : BaseRepository<RefreshToken>(dbContext), IRefreshTokenRepository
    {
    }
}
