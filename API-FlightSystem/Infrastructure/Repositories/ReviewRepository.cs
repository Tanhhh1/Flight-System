using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistences;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class ReviewRepository(DatabaseContext dbContext) : BaseRepository<Review>(dbContext), IReviewRepository
    {
    }
}
