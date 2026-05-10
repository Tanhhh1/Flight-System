using Application.Interfaces.Repositories;

namespace Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        IAirportRepository AirportRepository { get; }
        IAirlineRepository AirlineRepository { get; }
        IPlaneRepository PlaneRepository { get; }
        IRouteRepository RouteRepository { get; }
    }
}
