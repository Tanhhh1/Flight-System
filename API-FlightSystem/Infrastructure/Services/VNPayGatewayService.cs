using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class VNPayGatewayService : IPaymentGateway
    {
        private readonly IConfiguration _config;

        public VNPayGatewayService(IConfiguration config)
        {
            _config = config;
        }

        public PaymentMethod Method => PaymentMethod.VNPay;

        public Task<PaymentUrlResult> CreatePaymentUrlAsync(PaymentRequest request)
        {
            var vnpConfig = _config.GetSection("VNPay");
            var transactionId = $"VNP{DateTime.UtcNow.Ticks}";

            var queryParams = new SortedDictionary<string, string>
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = vnpConfig["TmnCode"]!,
                ["vnp_Amount"] = ((long)(request.Amount * 100)).ToString(),
                ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = "VND",
                ["vnp_IpAddr"] = request.IpAddress,
                ["vnp_Locale"] = "vn",
                ["vnp_OrderInfo"] = request.BookingCode,
                ["vnp_OrderType"] = "other",
                ["vnp_ReturnUrl"] = request.ReturnUrl,
                ["vnp_TxnRef"] = transactionId,
            };

            var queryString = string.Join("&", queryParams.Select(k => $"{k.Key}={k.Value}"));
            var paymentUrl = $"{vnpConfig["PaymentUrl"]}?{queryString}&vnp_SecureHash=MOCK_HASH";

            return Task.FromResult(new PaymentUrlResult(
                Success: true,
                PaymentUrl: paymentUrl,
                TransactionId: transactionId
            ));
        }

        public Task<PaymentCallbackResult> ProcessCallbackAsync(Dictionary<string, string> parameters)
        {
            var isSuccess = parameters.GetValueOrDefault("vnp_ResponseCode") == "00";
            var transId = parameters.GetValueOrDefault("vnp_TxnRef", "");
            var bookingCode = parameters.GetValueOrDefault("vnp_OrderInfo", "");
            var amount = long.TryParse(parameters.GetValueOrDefault("vnp_Amount", "0"), out var raw)
                                ? raw / 100m : 0m;

            return Task.FromResult(new PaymentCallbackResult(
                IsSuccess: isSuccess,
                TransactionId: transId,
                BookingCode: bookingCode,
                Amount: amount,
                RawData: System.Text.Json.JsonSerializer.Serialize(parameters)
            ));
        }
    }
}