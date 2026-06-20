using Application.Hubs;
using Application.Interfaces.Hubs;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services
{
    public class SeatNotificationService : ISeatNotificationService
    {
        private readonly IHubContext<SeatHub, ISeatHubClient> _hubContext;

        public SeatNotificationService(IHubContext<SeatHub, ISeatHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySeatLockedAsync(int flightId, int flightSeatId, int seatId, int passengerId)
        {
            await _hubContext.Clients
                .Group($"flight-{flightId}")
                .SeatStatusChanged(new
                {
                    flightId,
                    flightSeatId,
                    seatId,
                    status = (int)SeatStatus.Locked,
                    lockedByPassengerId = passengerId,
                });
        }

        public async Task NotifySeatsReleasedAsync(int flightId, List<int> flightSeatIds, List<int> seatIds)
        {
            await _hubContext.Clients
                .Group($"flight-{flightId}")
                .SeatStatusChanged(new
                {
                    flightId,
                    flightSeatIds,
                    seatIds,
                    status = (int)SeatStatus.Available,
                    lockedByPassengerId = (int?)null,
                });
        }

        public async Task NotifySeatBookedAsync(int flightId, int flightSeatId, int seatId)
        {
            await _hubContext.Clients
                .Group($"flight-{flightId}")
                .SeatStatusChanged(new
                {
                    flightId,
                    flightSeatId,
                    seatId,
                    status = (int)SeatStatus.Booked,
                    lockedByPassengerId = (int?)null,
                });
        }
    }
}