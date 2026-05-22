namespace Application.Common.Caching
{
    public class CacheProfile
    {
        public string Prefix { get; }
        public int TTL { get; }

        public CacheProfile(string prefix, int ttl)
        {
            Prefix = prefix;
            TTL = ttl;
        }

        public static CacheProfile Of(string prefix, int ttl) => new(prefix, ttl);
    }
}
