using Application.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.Hubs
{
    [Authorize]
    public class SeatHub : Hub<ISeatHubClient>
    {
        public async Task JoinFlight(int flightId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"flight-{flightId}");
        }

        public async Task LeaveFlight(int flightId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"flight-{flightId}");
        }
    }
}