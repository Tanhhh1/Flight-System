namespace Application.Services
{
    public record BookingConfirmationEmailDto(
        string ToEmail,
        string CustomerName,
        string BookingCode,
        string TripType,
        string PaymentMethod,
        decimal TotalPrice,
        DateTime BookingDate
    );

    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(BookingConfirmationEmailDto dto);
    }
}