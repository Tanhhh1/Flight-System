using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class FlightStatusUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FlightStatusUpdateService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public FlightStatusUpdateService(IServiceScopeFactory scopeFactory, ILogger<FlightStatusUpdateService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateCompletedFlightsAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task UpdateCompletedFlightsAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var now = DateTime.UtcNow;
                var flights = await unitOfWork.FlightRepository
                    .GetByCondition(f => (f.Status == FlightStatus.Active || f.Status == FlightStatus.Delayed) && f.ArrivalTime < now)
                    .ToListAsync(cancellationToken);
                if (!flights.Any())
                {
                    _logger.LogInformation("No flights to update at {Time}", now);
                    return;
                }
                foreach (var flight in flights)
                {
                    flight.Status = FlightStatus.Completed;
                    unitOfWork.FlightRepository.Update(flight);
                }
                await unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} flights to Completed at {Time}", flights.Count, now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating flight statuses.");
            }
        }
    }
}