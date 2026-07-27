using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class VNPayService : IVNPayService
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _baseUrl;
        private readonly string _version;
        private readonly string _command;
        private readonly string _currCode;
        private readonly string _locale;

        private static readonly Dictionary<PaymentMethod, string> BankCodeMap = new()
        {
            { PaymentMethod.DomesticCard, "VNBANK" },
            { PaymentMethod.InternationalCard, "INTCARD" },
        };

        public VNPayService(IConfiguration configuration)
        {
            var section = configuration.GetSection("VNPay");
            _tmnCode = section["TmnCode"] ?? throw new InvalidOperationException("VNPay: TmnCode is not configured");
            _hashSecret = section["HashSecret"] ?? throw new InvalidOperationException("VNPay: HashSecret is not configured");
            _baseUrl = section["BaseUrl"] ?? throw new InvalidOperationException("VNPay: BaseUrl is not configured");
            _version = section["Version"] ?? "2.1.0";
            _command = section["Command"] ?? "pay";
            _currCode = section["CurrCode"] ?? "VND";
            _locale = section["Locale"] ?? "vn";
        }

        public Task<VNPayUrlResult> CreatePaymentUrlAsync(VNPayUrlRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var now = DateTime.UtcNow.AddHours(7);
                var createDate = now.ToString("yyyyMMddHHmmss");
                var expireDate = now.AddMinutes(15).ToString("yyyyMMddHHmmss");
                var txnRef = $"{request.BookingCode}_{now:HHmmss}";
                var amount = ((long)(request.Amount * 100)).ToString();

                BankCodeMap.TryGetValue(request.Method, out var bankCode);

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
                {
                    params_["vnp_BankCode"] = bankCode;
                }

                var queryString = BuildQueryString(params_);
                var secureHash = ComputeHmacSha512(_hashSecret, queryString);
                var paymentUrl = $"{_baseUrl}?{queryString}&vnp_SecureHash={secureHash}";

                return Task.FromResult(new VNPayUrlResult
                {
                    IsSuccess = true,
                    PaymentUrl = paymentUrl,
                    TxnRef = txnRef
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new VNPayUrlResult
                {
                    IsSuccess = false,
                    PaymentUrl = string.Empty,
                    TxnRef = string.Empty,
                    ErrorMessage = ex.Message
                });
            }
        }

        public Task<VNPayCallback> ProcessCallbackAsync(IDictionary<string, string> queryParameters, CancellationToken cancellationToken = default)
        {
            queryParameters.TryGetValue("vnp_SecureHash", out var receivedHash);

            var signableParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var kvp in queryParameters)
            {
                if (kvp.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase) &&
                    !kvp.Key.Equals("vnp_SecureHash", StringComparison.OrdinalIgnoreCase) &&
                    !kvp.Key.Equals("vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                {
                    signableParams[kvp.Key] = kvp.Value;
                }
            }

            var queryString = BuildQueryString(signableParams);
            var computedHash = ComputeHmacSha512(_hashSecret, queryString);
            var isValidHash = string.Equals(computedHash, receivedHash, StringComparison.OrdinalIgnoreCase);

            queryParameters.TryGetValue("vnp_ResponseCode", out var responseCode);
            queryParameters.TryGetValue("vnp_TxnRef", out var txnRef);
            queryParameters.TryGetValue("vnp_TransactionNo", out var vnpTransactionNo);
            queryParameters.TryGetValue("vnp_Amount", out var amountStr);

            responseCode ??= "99";
            txnRef ??= string.Empty;
            var bookingCode = txnRef.Contains('_') ? txnRef[..txnRef.LastIndexOf('_')] : txnRef;

            var amount = decimal.TryParse(amountStr, out var rawAmount) ? rawAmount / 100 : 0;
            var isSuccess = isValidHash && responseCode == "00";

            return Task.FromResult(new VNPayCallback
            {
                IsSuccess = isSuccess,
                TxnRef = txnRef,
                VNPayTransactionId = vnpTransactionNo,
                BookingCode = bookingCode,
                Amount = amount,
                ResponseCode = responseCode,
                RawData = queryString
            });
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
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}