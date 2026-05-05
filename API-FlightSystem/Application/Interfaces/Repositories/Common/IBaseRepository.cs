using System.Linq.Expressions;

namespace Application.Interfaces.Repositories.Common
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        IQueryable<T> GetByCondition(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? order = null);
    }
}

