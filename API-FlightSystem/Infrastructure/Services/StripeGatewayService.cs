using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class StripeGatewayService : IPaymentGateway
    {
        private readonly IConfiguration _config;

        public StripeGatewayService(IConfiguration config)
        {
            _config = config;
        }

        public PaymentMethod Method => PaymentMethod.Stripe;

        public Task<PaymentUrlResult> CreatePaymentUrlAsync(PaymentRequest request)
        {
            var stripeConfig = _config.GetSection("Stripe");
            var transactionId = $"STR{DateTime.UtcNow.Ticks}";

            var queryParams = new Dictionary<string, string>
            {
                ["client_reference_id"] = request.BookingCode,
                ["amount"] = ((long)(request.Amount * 100)).ToString(),
                ["currency"] = "vnd",
                ["success_url"] = request.ReturnUrl,
                ["cancel_url"] = request.ReturnUrl,
                ["payment_intent"] = transactionId,
            };

            var queryString = string.Join("&", queryParams.Select(k => $"{k.Key}={k.Value}"));
            var paymentUrl = $"{stripeConfig["PaymentUrl"]}?{queryString}&mock_key={stripeConfig["PublishableKey"]}";

            return Task.FromResult(new PaymentUrlResult(
                Success: true,
                PaymentUrl: paymentUrl,
                TransactionId: transactionId
            ));
        }

        public Task<PaymentCallbackResult> ProcessCallbackAsync(Dictionary<string, string> parameters)
        {
            var isSuccess = parameters.GetValueOrDefault("status") == "succeeded";
            var transId = parameters.GetValueOrDefault("payment_intent", "");
            var bookingCode = parameters.GetValueOrDefault("client_reference_id", "");
            var amount = decimal.TryParse(parameters.GetValueOrDefault("amount", "0"), out var raw)
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