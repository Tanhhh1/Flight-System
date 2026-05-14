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
        IServiceRepository ServiceRepository { get; }
        IReviewRepository ReviewRepository { get; }
        IFlightRepository FlightRepository { get; }
        IPolicyRepository PolicyRepository { get; }
        IFlightServiceRepository FlightServiceRepository { get; }
        IFlightSegmentRepository FlightSegmentRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        ISeatTemplateRepository SeatTemplateRepository { get; }
        IBookingRepository BookingRepository { get; }
    }
}
