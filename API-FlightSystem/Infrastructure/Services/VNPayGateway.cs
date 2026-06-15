using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class VNPayGateway : IPaymentGateway
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _baseUrl;
        private readonly string _version;
        private readonly string _command;
        private readonly string _currCode;
        private readonly string _locale;

        private static readonly Dictionary<PaymentMethod, string> _bankCodeMap = new()
        {
            { PaymentMethod.DomesticCard, "VNBANK" },
            { PaymentMethod.InternationalCard, "INTCARD" },
        };

        public VNPayGateway(IConfiguration configuration)
        {
            var section = configuration.GetSection("VNPay");
            _tmnCode = section["TmnCode"] ?? throw new Exception("VNPay TmnCode is not configured");
            _hashSecret = section["HashSecret"] ?? throw new Exception("VNPay HashSecret is not configured");
            _baseUrl = section["BaseUrl"] ?? throw new Exception("VNPay BaseUrl is not configured");
            _version = section["Version"] ?? "2.1.0";
            _command = section["Command"] ?? "pay";
            _currCode = section["CurrCode"] ?? "VND";
            _locale = section["Locale"] ?? "vn";
        }

        public Task<PaymentUrlResult> CreatePaymentUrlAsync(PaymentRequest request)
        {
            var now = DateTime.UtcNow.AddHours(7);
            var createDate = now.ToString("yyyyMMddHHmmss");
            var expireDate = now.AddMinutes(15).ToString("yyyyMMddHHmmss");
            var txnRef = $"{request.BookingCode}_{now:HHmmss}";
            var amount = ((long)(request.Amount * 100)).ToString();

            _bankCodeMap.TryGetValue(request.Method, out var bankCode);

            var params_ = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = _version,
                ["vnp_Command"] = _command,
                ["vnp_TmnCode"] = _tmnCode,
                ["vnp_Amount"] = amount,
                ["vnp_CurrCode"] = _currCode,
                ["vnp_TxnRef"] = txnRef,
                ["vnp_OrderInfo"] = request.Description,
                ["vnp_OrderType"] = "250000",
                ["vnp_Locale"] = _locale,
                ["vnp_ReturnUrl"] = request.ReturnUrl,
                ["vnp_IpAddr"] = request.IpAddress,
                ["vnp_CreateDate"] = createDate,
                ["vnp_ExpireDate"] = expireDate,
            };

            if (!string.IsNullOrEmpty(bankCode))
                params_["vnp_BankCode"] = bankCode;

            var queryString = BuildQueryString(params_);
            var secureHash = ComputeHmacSha512(_hashSecret, queryString);
            var paymentUrl = $"{_baseUrl}?{queryString}&vnp_SecureHash={secureHash}";

            return Task.FromResult(new PaymentUrlResult(
                Success: true,
                PaymentUrl: paymentUrl,
                TransactionId: txnRef
            ));
        }

        public Task<PaymentCallbackResult> ProcessCallbackAsync(Dictionary<string, string> parameters)
        {
            parameters.TryGetValue("vnp_SecureHash", out var receivedHash);

            var signableParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var kvp in parameters)
            {
                if (kvp.Key.StartsWith("vnp_") && kvp.Key != "vnp_SecureHash")
                    signableParams[kvp.Key] = kvp.Value;
            }

            var queryString = BuildQueryString(signableParams);
            var computedHash = ComputeHmacSha512(_hashSecret, queryString);
            var isValidHash = string.Equals(computedHash, receivedHash, StringComparison.OrdinalIgnoreCase);

            parameters.TryGetValue("vnp_ResponseCode", out var responseCode);
            parameters.TryGetValue("vnp_TxnRef", out var txnRef);
            parameters.TryGetValue("vnp_Amount", out var amountStr);
            parameters.TryGetValue("vnp_OrderInfo", out var orderInfo);

            var bookingCode = txnRef?.Contains("_") == true ? txnRef[..txnRef.LastIndexOf('_')] : txnRef ?? string.Empty;

            var amount = decimal.TryParse(amountStr, out var a) ? a / 100 : 0;
            var isSuccess = isValidHash && responseCode == "00";

            return Task.FromResult(new PaymentCallbackResult(
                IsSuccess: isSuccess,
                TransactionId: txnRef ?? string.Empty,
                BookingCode: bookingCode,
                Amount: amount,
                RawData: queryString
            ));
        }

        private static string BuildQueryString(SortedDictionary<string, string> params_)
        {
            var parts = params_.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}");
            return string.Join("&", parts);
        }

        private static string ComputeHmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}