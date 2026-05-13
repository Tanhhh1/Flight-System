using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Shared.Helpers
{
    public static class StringHelper
    {
        public static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string NormalizeString(this string input, string replacement = " ")
        {
            return string.IsNullOrWhiteSpace(input) ? "" : Regex.Replace(input.Trim(), @"\s+", replacement);
        }
    }
}
