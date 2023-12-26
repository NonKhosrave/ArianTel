using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Contract;
using BuildingBlocks.Logger;
using BuildingBlocks.Model;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Service;

public sealed class Retry : IRetry
{
    private readonly ILogger<Retry> _logger;

    public Retry(ILogger<Retry> logger)
    {
        _logger = logger;
    }

    public void Do(
        Action action,
        TimeSpan retryInterval,
        int retryCount)
    {
        Do<object>(() =>
        {
            action();
            return null;
        }, retryInterval, retryCount);
    }

    public T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount)
    {
        var exceptions = new List<Exception>();

        for (var retry = 0; retry < retryCount; retry++)
            try
            {
                if (retry > 0)
                    Thread.Sleep(retryInterval);

                return action();
            }
            catch (Exception ex)
            {
                _logger.CompileLog(ex, LogLevel.Error, ex.Message);

                exceptions.Add(ex);
            }

        throw new AggregateException(exceptions);
    }

    public async Task<RetryResponse<T>> DoWithShallowAsync<T>(Func<Task<T>> action, TimeSpan retryInterval,
        int retryCount, bool configureAwait = true)
    {
        var exceptions = new List<Exception>();

        for (var retry = 0; retry < retryCount; retry++)
            try
            {
                if (retry > 0)
                    await Task.Delay(retryInterval).ConfigureAwait(false);
                return new RetryResponse<T>
                {
                    Result = await action().ConfigureAwait(configureAwait),
                    IsSuccessful = true
                };
            }
            catch (Exception ex)
            {
                _logger.CompileLog(ex, LogLevel.Error, ex.Message);

                exceptions.Add(ex);
            }

        return new RetryResponse<T> { IsSuccessful = false, Exception = new AggregateException(exceptions) };
    }
}
