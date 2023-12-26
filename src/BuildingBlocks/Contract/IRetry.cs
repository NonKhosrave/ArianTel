using System;
using System.Threading.Tasks;
using BuildingBlocks.Model;

namespace BuildingBlocks.Contract;
public interface IRetry
{
    void Do(Action action, TimeSpan retryInterval, int retryCount);

    T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount);

    Task<RetryResponse<T>> DoWithShallowAsync<T>(Func<Task<T>> action, TimeSpan retryInterval, int retryCount,
        bool configureAwait = true);
}
