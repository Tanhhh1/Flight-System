using Application.Interfaces.Repositories.Common;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DatabaseContext _dbContext;

        public BaseRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IQueryable<T> GetByCondition(
            Expression<Func<T, bool>>? expression = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>>? order = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (expression != null)
                query = query.Where(expression);
            if (order != null)
                query = order(query);
            return query;
        }
    }
}
