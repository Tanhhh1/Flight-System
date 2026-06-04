using Application.Interfaces.Hubs;
using Application.Interfaces.UnitOfWork;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Hubs
{
    public class SeatHub : Hub<ISeatHubClient>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SeatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task JoinFlightGroup(int flightId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(flightId));
        }
        public async Task LeaveFlightGroup(int flightId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(flightId));
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
                var lockedSeats = await _unitOfWork.FlightSeatRepository
                    .GetByCondition(s => s.Status == SeatStatus.Locked && s.LockedBy == userId)
                    .Include(s => s.SeatTemplate)
                    .ToListAsync();
                if (lockedSeats.Any())
                {
                    foreach (var seat in lockedSeats)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.LockedUntil = default;
                        seat.LockedBy = 0;
                        _unitOfWork.FlightSeatRepository.Update(seat);
                        await Clients
                            .Group(GetGroupName(seat.FlightId))
                            .SeatUnlocked(seat.FlightSeatId, seat.SeatTemplate.SeatNumber);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
        public static string GetGroupName(int flightId) => $"flight-{flightId}";
    }
}