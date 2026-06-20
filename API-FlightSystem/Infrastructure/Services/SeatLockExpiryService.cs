using Application.Interfaces.Hubs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class SeatLockExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SeatLockExpiryService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public SeatLockExpiryService(
            IServiceScopeFactory scopeFactory,
            ILogger<SeatLockExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);
                await ProcessExpiredLocksAsync(stoppingToken);
            }
        }

        private async Task ProcessExpiredLocksAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var seatNotification = scope.ServiceProvider.GetRequiredService<ISeatNotificationService>();

                var expiredSeats = await unitOfWork.FlightSeatRepository
                    .GetByCondition(fs =>
                        fs.Status == SeatStatus.Locked &&
                        fs.LockedUntil < DateTime.UtcNow)
                    .ToListAsync(cancellationToken);

                if (!expiredSeats.Any()) return;

                await unitOfWork.BeginTransactionAsync();
                try
                {
                    foreach (var seat in expiredSeats)
                        unitOfWork.FlightSeatRepository.Delete(seat);

                    await unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                _logger.LogInformation("Da tu dong xaa {Count} ghe het han giu luc {Time}", expiredSeats.Count, DateTime.UtcNow);
                var groupedByFlight = expiredSeats.GroupBy(s => s.FlightId);
                foreach (var flightGroup in groupedByFlight)
                {
                    int flightId = flightGroup.Key;
                    var flightSeatIds = flightGroup.Select(s => s.FlightSeatId).ToList();
                    var seatIds = flightGroup.Select(s => s.SeatId).ToList();
                    _ = seatNotification.NotifySeatsReleasedAsync(flightId, flightSeatIds, seatIds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Loi khi xu ly expired seat locks.");
            }
        }
    }
}