using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class PassengerTypeRepository(DatabaseContext dbContext) : BaseRepository<PassengerType>(dbContext), IPassengerTypeRepository
    {
    }
}
