using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class SeatTemplateRepository(DatabaseContext dbContext) : BaseRepository<SeatTemplate>(dbContext), ISeatTemplateRepository
    {
    }
}
