using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class MoMoGatewayService : IPaymentGateway
    {
        private readonly IConfiguration _config;

        public MoMoGatewayService(IConfiguration config)
        {
            _config = config;
        }

        public PaymentMethod Method => PaymentMethod.MoMo;

        public Task<PaymentUrlResult> CreatePaymentUrlAsync(PaymentRequest request)
        {
            var momoConfig = _config.GetSection("MoMo");
            var transactionId = $"MOMO{DateTime.UtcNow.Ticks}";

            var queryParams = new Dictionary<string, string>
            {
                ["partnerCode"] = momoConfig["PartnerCode"]!,
                ["orderId"] = transactionId,
                ["orderInfo"] = request.BookingCode,
                ["amount"] = ((long)request.Amount).ToString(),
                ["returnUrl"] = request.ReturnUrl,
                ["requestId"] = Guid.NewGuid().ToString(),
            };

            var queryString = string.Join("&", queryParams.Select(k => $"{k.Key}={k.Value}"));
            var paymentUrl = $"{momoConfig["PaymentUrl"]}?{queryString}&signature=MOCK_SIGNATURE";

            return Task.FromResult(new PaymentUrlResult(
                Success: true,
                PaymentUrl: paymentUrl,
                TransactionId: transactionId
            ));
        }

        public Task<PaymentCallbackResult> ProcessCallbackAsync(Dictionary<string, string> parameters)
        {
            var isSuccess = parameters.GetValueOrDefault("resultCode") == "0";
            var transId = parameters.GetValueOrDefault("orderId", "");
            var bookingCode = parameters.GetValueOrDefault("orderInfo", "");
            var amount = decimal.TryParse(parameters.GetValueOrDefault("amount", "0"), out var raw)
                                ? raw : 0m;

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