using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class BookingRepository(DatabaseContext dbContext) : BaseRepository<Booking>(dbContext), IBookingRepository
    {
    }
}
