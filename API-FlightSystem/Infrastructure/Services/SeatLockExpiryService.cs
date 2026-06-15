using Application.Hubs;
using Application.Interfaces.Hubs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class SeatLockExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<SeatHub, ISeatHubClient> _hubContext;
        private readonly ILogger<SeatLockExpiryService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public SeatLockExpiryService(IServiceScopeFactory scopeFactory, IHubContext<SeatHub, ISeatHubClient> hubContext, ILogger<SeatLockExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
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
                var expiredSeats = await unitOfWork.FlightSeatRepository
                    .GetByCondition(fs => fs.Status == SeatStatus.Locked && fs.LockedUntil < DateTime.UtcNow)
                    .ToListAsync(cancellationToken);

                if (!expiredSeats.Any()) return;

                await unitOfWork.BeginTransactionAsync();

                try
                {
                    foreach (var seat in expiredSeats)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.LockedBy = 0;
                        seat.LockedUntil = DateTime.MinValue;
                        unitOfWork.FlightSeatRepository.Update(seat);
                    }
                    await unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                _logger.LogInformation("Đã tự động unlock {Count} ghế hết hạn lúc {Time}", expiredSeats.Count, DateTime.UtcNow);
                var seatsByFlight = expiredSeats.GroupBy(s => s.FlightId);

                foreach (var flightGroup in seatsByFlight)
                {
                    var broadcastTasks = flightGroup.Select(seat =>
                        _hubContext.Clients
                            .Group($"flight-{flightGroup.Key}")
                            .SeatStatusChanged(new
                            {
                                flightSeatId = seat.FlightSeatId,
                                seatId = seat.SeatId,
                                status = SeatStatus.Available,
                                lockedByPassengerId = (int?)null,
                            }));

                    await Task.WhenAll(broadcastTasks);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý expired seat locks.");
            }
        }
    }
}