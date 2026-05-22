using Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching
{
    public class MemoryCacheService : IMemoryCacheService, IDisposable
    {
        private readonly MemoryCache _cache;
        private readonly Dictionary<string, HashSet<string>> _keysByPrefix = new();
        private readonly object _lock = new();

        public MemoryCacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGet<T>(string key, out T? value)
        {
            if (_cache.TryGetValue(key, out var raw) && raw is T typed)
            {
                value = typed;
                return true;
            }
            value = default;
            return false;
        }

        public void Set<T>(string key, T value, TimeSpan duration, string prefix)
        {
            var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(duration).SetPriority(CacheItemPriority.Normal);
            _cache.Set(key, (object)value!, options);
            lock (_lock)
            {
                if (!_keysByPrefix.ContainsKey(prefix))
                    _keysByPrefix[prefix] = new HashSet<string>();
                _keysByPrefix[prefix].Add(key);
            }
        }

        public void RemoveByPrefix(string prefix)
        {
            lock (_lock)
            {
                var matched = _keysByPrefix.Keys.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var key in matched)
                {
                    foreach (var k in _keysByPrefix[key])
                        _cache.Remove(k);
                    _keysByPrefix.Remove(key);
                }
            }
        }

        public void Dispose() => _cache.Dispose();
    }
}