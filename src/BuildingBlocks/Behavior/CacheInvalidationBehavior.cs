using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using BuildingBlocks.Service.Cache;
using MediatR;

namespace BuildingBlocks.Behavior;
public sealed class CacheInvalidationBehavior<TRequest, TResponse> : BehaviorBase<TRequest, TResponse>
    where TRequest : BaseEntity, IRequest<TResponse>, ICacheInvalidatorRequest
    where TResponse : ServiceResContextBase, new()
{
    private readonly ICustomDistributeCache _customDistributeCache;

    public CacheInvalidationBehavior(ICustomDistributeCache customDistributeCache)
    {
        _customDistributeCache = customDistributeCache;
    }

    protected override async Task<TResponse> HandleCore(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request is ICacheInvalidatorRequest invalidator)
        {
            if (string.IsNullOrEmpty(invalidator.CacheKey))
                throw new ArgumentNullException(nameof(request), "null cache key");

            var response = await next().ConfigureAwait(false);

            if (!response.HasError)
                await _customDistributeCache.RemoveAsync(invalidator.CacheKey, cancellationToken).ConfigureAwait(false);

            return response;
        }

        return await next().ConfigureAwait(false);
    }
}
