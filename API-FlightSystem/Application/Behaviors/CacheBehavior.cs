using Application.Common.Caching;
using Application.Interfaces.CQRS;
using Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors
{
    public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IMemoryCacheService _memory;
        private readonly ILogger<CacheBehavior<TRequest, TResponse>> _logger;

        public CacheBehavior(IMemoryCacheService memory, ILogger<CacheBehavior<TRequest, TResponse>> logger)
        {
            _memory = memory;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICacheable cacheable)
                return await next();

            var profile = cacheable.GetCacheProfile();
            var key = CacheKey.Generate(request, profile.Prefix);
            var ttl = TimeSpan.FromMinutes(profile.TTL);

            // HIT
            if (_memory.TryGet<TResponse>(key, out var cached) && cached != null)
            {
                _logger.LogInformation("[Cache HIT] Key: {Key}", key);
                return cached;
            }

            // MISS → DB
            _logger.LogInformation("[Cache MISS] Key: {Key} → query DB", key);
            var response = await next();

            _memory.Set(key, response, ttl, profile.Prefix);
            _logger.LogInformation("[Cache SET] Key: {Key} TTL: {TTL}m", key, profile.TTL);

            return response;
        }
    }
}