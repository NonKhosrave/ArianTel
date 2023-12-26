using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Service.Cache;
public interface ICustomDistributeCache
{
    byte[] Get(string key);

    Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default);

    void Set(string key, byte[] value, DistributedCacheEntryOptions options);

    void SetString(string key, string value, DistributedCacheEntryOptions options);

    Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);

    void Refresh(string key);

    Task RefreshAsync(string key, CancellationToken cancellationToken = default);

    void Remove(string key);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task SetAsync(string key, string value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default);

    Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default);

    Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default);
}
