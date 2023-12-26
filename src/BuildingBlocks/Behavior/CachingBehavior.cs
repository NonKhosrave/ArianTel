using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Service.Cache;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace BuildingBlocks.Behavior;
public sealed class CachingBehavior<TRequest, TResponse> : BehaviorBase<TRequest, TResponse>
    where TRequest : BaseEntity, IRequest<TResponse>, ICacheableRequest<TResponse>
    where TResponse : ServiceResContextBase, new()
{
    private readonly IMemoryCache _memoryCache;
    private readonly ICustomDistributeCache _customDistributeCache;

    public CachingBehavior(ICustomDistributeCache customDistributeCache, IMemoryCache memoryCache)
    {
        _customDistributeCache = customDistributeCache;
        _memoryCache = memoryCache;
    }

    protected override async Task<TResponse> HandleCore(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is ICacheableRequest<TResponse> cacheable)
        {
            if (string.IsNullOrEmpty(cacheable.CacheKey))
                throw new ArgumentNullException(nameof(request), "null cache key");

            TResponse response;
            if (!cacheable.UseMemoryCache)
            {
                response = await _customDistributeCache.GetAsync<TResponse>(cacheable.CacheKey, cancellationToken).ConfigureAwait(false);

                if (response is not null) return response;

                response = await next().ConfigureAwait(false);

                if (response is not null && !response.HasError && (cacheable.ConditionFroSetCache is null || cacheable.ConditionFroSetCache(response)))
                    await _customDistributeCache.SetAsync(cacheable.CacheKey, response, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = cacheable.ConditionExpiration(response)
                    }, cancellationToken).ConfigureAwait(false);

                return response;
            }

            response = _memoryCache.Get<TResponse>(cacheable.CacheKey);

            if (response is not null) return response;

            response = await next().ConfigureAwait(false);

            if (response is not null && !response.HasError && (cacheable.ConditionFroSetCache is null || cacheable.ConditionFroSetCache(response)))
                _memoryCache.Set(cacheable.CacheKey, response, cacheable.ConditionExpiration(response));

            return response;
        }

        return await next().ConfigureAwait(false);
    }
}
