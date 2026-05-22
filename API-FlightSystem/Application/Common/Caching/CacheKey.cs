using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Application.Common.Caching
{
    public static class CacheKey
    {
        public static string Generate<T>(T request, string prefix)
        {
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = false
            });
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return $"{prefix}:{Convert.ToHexString(hash)[..16]}";
        }
    }
}