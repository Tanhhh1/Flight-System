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
        private IServiceRepository? _serviceRepository;
        private IReviewRepository? _reviewRepository;
        private IFlightRepository? _flightRepository;
        private IPolicyRepository? _policyRepository;
        private IFlightServiceRepository? _flightServiceRepository;
        private IFlightSegmentRepository? _flightSegmentRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;
        public UnitOfWork(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IAirportRepository AirportRepository => _airportRepository ??= new AirportRepository(_dbContext);
        public IAirlineRepository AirlineRepository => _airlineRepository ??= new AirlineRepository(_dbContext);
        public IPlaneRepository PlaneRepository => _planeRepository ??= new PlaneRepository(_dbContext);
        public IRouteRepository RouteRepository => _routeRepository ??= new RouteRepository(_dbContext);
        public IServiceRepository ServiceRepository => _serviceRepository ??= new ServiceRepository(_dbContext);
        public IReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(_dbContext);
        public IFlightRepository FlightRepository => _flightRepository ??= new FlightRepository(_dbContext);
        public IPolicyRepository PolicyRepository => _policyRepository ??= new PolicyRepository(_dbContext);
        public IFlightServiceRepository FlightServiceRepository => _flightServiceRepository ??= new FlightServiceRepository(_dbContext);
        public IFlightSegmentRepository FlightSegmentRepository => _flightSegmentRepository ??= new FlightSegmentRepository(_dbContext);
        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(_dbContext);

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
