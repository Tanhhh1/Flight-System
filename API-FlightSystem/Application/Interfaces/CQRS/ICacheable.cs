using Application.Common.Caching;

namespace Application.Interfaces.CQRS
{
    public interface ICacheable
    {
        CacheProfile GetCacheProfile();
    }
}
