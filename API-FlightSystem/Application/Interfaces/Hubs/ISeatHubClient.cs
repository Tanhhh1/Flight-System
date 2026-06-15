namespace Application.Interfaces.Hubs
{
    public interface ISeatHubClient
    {
        Task SeatStatusChanged(object payload);
    }
}