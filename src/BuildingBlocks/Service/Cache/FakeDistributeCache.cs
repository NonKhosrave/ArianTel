using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Service.Cache;
public sealed class FakeDistributeCache : ICustomDistributeCache
{
#pragma warning disable S1168 // Empty arrays and collections should be returned instead of null
    public byte[] Get(string key) => null;
#pragma warning restore S1168 // Empty arrays and collections should be returned instead of null

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        // Method intentionally left empty.
    }

    public void SetString(string key, string value, DistributedCacheEntryOptions options)
    {
        // Method intentionally left empty.
    }

    public void Refresh(string key)
    {
        // Method intentionally left empty.
    }

    public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        // Method intentionally left empty.
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, string value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<string>(default);
    }

    public Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<TEntity>(default);
    }

    public Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<byte[]>(null);
    }

    public Task SetAsync(string key, object value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
