using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Logger;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Service.Cache;
public sealed class CustomDistributeCache : ICustomDistributeCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CustomDistributeCache> _logger;

    public CustomDistributeCache(IDistributedCache distributedCache, ILogger<CustomDistributeCache> logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public byte[] Get(string key)
    {
        try
        {
            var bytes = _distributedCache.Get(key);

            return bytes?.Lz4Decompress();
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);

            return null;
        }
    }

    public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await _distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
            return bytes?.Lz4Decompress();
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);

            return null;
        }
    }

    public async Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var itemBytes = await _distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
            if (itemBytes == null)
                return default;

            var lz4Decompress = itemBytes.Lz4Decompress();
            var itemString = Encoding.UTF8.GetString(lz4Decompress);
            return itemString.ToObject<TEntity>();
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);

            return default;
        }
    }

    public async Task<string> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var itemBytes = await _distributedCache.GetAsync(key, cancellationToken).ConfigureAwait(false);
            if (itemBytes == null)
                return default;

            var lz4Decompress = itemBytes.Lz4Decompress();
            return Encoding.UTF8.GetString(lz4Decompress);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);

            return default;
        }
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        try
        {
            var bytesCom = value.Lz4Compress();
            _distributedCache.Set(key, bytesCom, options);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }
    }

    public void SetString(string key, string value, DistributedCacheEntryOptions options)
    {
        Set(key, Encoding.UTF8.GetBytes(value), options);
    }

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bytesCom = value.Lz4Compress();
            await _distributedCache.SetAsync(key, bytesCom, options, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        await SetAsync(key, value.ToJsonUtf8Bytes(), options, cancellationToken).ConfigureAwait(false);
    }

    public async Task SetAsync(string key, string value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        await SetAsync(key, Encoding.UTF8.GetBytes(value), options, cancellationToken).ConfigureAwait(false);
    }

    public void Refresh(string key)
    {
        try
        {
            _distributedCache.Refresh(key);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }
    }

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RefreshAsync(key, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }
    }

    public void Remove(string key)
    {
        try
        {
            _distributedCache.Remove(key);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.CompileLog(e, LogLevel.Error, e.Message);
        }
    }
}
