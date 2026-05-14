using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class FlightRepository(DatabaseContext dbContext) : BaseRepository<Flight>(dbContext), IFlightRepository
    {
    }
}
