namespace Application.Interfaces.Hubs
{
    public interface ISeatHubClient
    {
        Task SeatLocked(int flightSeatId, string seatNumber, int lockedBy);
        Task SeatUnlocked(int flightSeatId, string seatNumber);
        Task SeatBooked(int flightSeatId, string seatNumber);
    }
}
