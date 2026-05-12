using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class ServiceRepository(DatabaseContext dbContext) : BaseRepository<Service>(dbContext), IServiceRepository
    {
    }
}
