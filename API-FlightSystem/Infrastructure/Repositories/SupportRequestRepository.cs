using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class SupportRequestRepository(DatabaseContext dbContext) : BaseRepository<SupportRequest>(dbContext), ISupportRequestRepository
    {
    }
}
