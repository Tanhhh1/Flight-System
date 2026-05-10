using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class PlaneRepository(DatabaseContext dbContext) : BaseRepository<Plane>(dbContext), IPlaneRepository
    {
    }
}
