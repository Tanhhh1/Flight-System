namespace Application.Interfaces.Hubs
{
    public interface ISeatNotificationService
    {
        Task NotifySeatLockedAsync(int flightId, int flightSeatId, int seatId, int passengerId);
        Task NotifySeatsReleasedAsync(int flightId, List<int> flightSeatIds, List<int> seatIds); 
        Task NotifySeatBookedAsync(int flightId, int flightSeatId, int seatId);
    }
}
