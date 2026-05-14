using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class FlightServiceRepository(DatabaseContext dbContext) : BaseRepository<FlightService>(dbContext), IFlightServiceRepository
    {
    }
}
