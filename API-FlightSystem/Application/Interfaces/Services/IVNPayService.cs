using Domain.Enums;

namespace Application.Interfaces.Services
{
    public class VNPayUrlRequest
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public PaymentMethod Method { get; set; }
    }

    public class VNPayUrlResult
    {
        public bool IsSuccess { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string TxnRef { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public class VNPayCallback
    {
        public bool IsSuccess { get; set; }
        public string TxnRef { get; set; } = string.Empty;
        public string? VNPayTransactionId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
    }

    public interface IVNPayService
    {
        Task<VNPayUrlResult> CreatePaymentUrlAsync(VNPayUrlRequest request, CancellationToken cancellationToken = default);
        Task<VNPayCallback> ProcessCallbackAsync(IDictionary<string, string> queryParameters, CancellationToken cancellationToken = default);
    }
}