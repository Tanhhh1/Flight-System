using Domain.Enums;

namespace Application.Interfaces.Services
{
    public record PaymentRequest(
        int BookingId,
        string BookingCode,
        decimal Amount,
        string Description,
        string ReturnUrl,
        string IpAddress,
        PaymentMethod Method 
    );

    public record PaymentUrlResult(
        bool Success,
        string PaymentUrl,
        string TransactionId,
        string? ErrorMessage = null
    );

    public record PaymentCallbackResult(
        bool IsSuccess,
        string TransactionId,
        string BookingCode,
        decimal Amount,
        string RawData
    );

    public interface IPaymentGateway
    {
        Task<PaymentUrlResult> CreatePaymentUrlAsync(PaymentRequest request);
        Task<PaymentCallbackResult> ProcessCallbackAsync(Dictionary<string, string> parameters);
    }
}