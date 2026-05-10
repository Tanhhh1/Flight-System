using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Infrastructure.Persistences;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;


namespace Infrastructure.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _dbContext;
        private IDbContextTransaction? _transaction;
        private IAirportRepository? _airportRepository;
        private IAirlineRepository? _airlineRepository;
        private IPlaneRepository? _planeRepository;
        private IRouteRepository? _routeRepository;

        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IAirportRepository AirportRepository => _airportRepository ??= new AirportRepository(_dbContext);
        public IAirlineRepository AirlineRepository => _airlineRepository ??= new AirlineRepository(_dbContext);
        public IPlaneRepository PlaneRepository => _planeRepository ??= new PlaneRepository(_dbContext);
        public IRouteRepository RouteRepository => _routeRepository ??= new RouteRepository(_dbContext);

        public async Task BeginTransactionAsync()
        {
            if (_dbContext.Database.CurrentTransaction == null)
            {
                _transaction = await _dbContext.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                if (_transaction != null) await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext.Dispose();
        }
    }
}
