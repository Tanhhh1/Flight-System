using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class AirportRepository(DatabaseContext dbContext) : BaseRepository<Airport>(dbContext), IAirportRepository
    {
    }
}
