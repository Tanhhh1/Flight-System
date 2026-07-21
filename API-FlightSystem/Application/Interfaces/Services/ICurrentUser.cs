namespace Application.Interfaces.Services
{
    public interface ICurrentUser
    {
        int? Id { get; }
        string? Username { get; }
        string? Fullname { get; }
        bool IsAuthenticated { get; }
    }
}
