using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class FlightSeatPriceRepository(DatabaseContext dbContext) : BaseRepository<FlightSeatPrice>(dbContext), IFlightSeatPriceRepository
    {
    }
}
