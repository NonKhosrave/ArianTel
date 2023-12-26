using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BuildingBlocks.Common;
public interface ICustomPublisher
{
    Task Publish<TNotification>(TNotification notification);

    Task Publish<TNotification>(TNotification notification, PublishStrategy strategy);

    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken);

    Task Publish<TNotification>(TNotification notification, PublishStrategy strategy,
        CancellationToken cancellationToken);
}

public sealed class CustomPublisher : ICustomPublisher
{
#pragma warning disable S1450
    private readonly IServiceProvider _serviceFactory;
#pragma warning restore S1450

    private readonly IDictionary<PublishStrategy, IMediator> _publishStrategies = new Dictionary<PublishStrategy, IMediator>();

    public CustomPublisher(IServiceProvider serviceFactory)
    {
        _serviceFactory = serviceFactory;

        _publishStrategies[PublishStrategy.Async] = new CustomMediator(_serviceFactory, AsyncContinueOnException);
        _publishStrategies[PublishStrategy.ParallelNoWait] = new CustomMediator(_serviceFactory, ParallelNoWait);
        _publishStrategies[PublishStrategy.ParallelWhenAll] = new CustomMediator(_serviceFactory, ParallelWhenAll);
        _publishStrategies[PublishStrategy.ParallelWhenAny] = new CustomMediator(_serviceFactory, ParallelWhenAny);
        _publishStrategies[PublishStrategy.SyncContinueOnException] =
            new CustomMediator(_serviceFactory, SyncContinueOnException);
        _publishStrategies[PublishStrategy.SyncStopOnException] =
            new CustomMediator(_serviceFactory, SyncStopOnException);
    }

    public PublishStrategy DefaultStrategy { get; set; } = PublishStrategy.SyncContinueOnException;

    public Task Publish<TNotification>(TNotification notification)
    {
        return Publish(notification, DefaultStrategy, default);
    }

    public Task Publish<TNotification>(TNotification notification, PublishStrategy strategy)
    {
        return Publish(notification, strategy, default);
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
    {
        return Publish(notification, DefaultStrategy, cancellationToken);
    }

    public Task Publish<TNotification>(TNotification notification, PublishStrategy strategy,
        CancellationToken cancellationToken)
    {
        if (!_publishStrategies.TryGetValue(strategy, out var mediator))
            throw new ArgumentException($"Unknown strategy: {strategy}", nameof(strategy));


        return mediator.Publish(notification, cancellationToken);
    }

    private Task ParallelWhenAll(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification,
        CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var handler in handlers)
            tasks.Add(Task.Run(() => handler.HandlerCallback(notification, cancellationToken), default));

        return Task.WhenAll(tasks);
    }

    private Task ParallelWhenAny(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification,
        CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var handler in handlers)
            tasks.Add(Task.Run(() => handler.HandlerCallback(notification, cancellationToken), default));

        return Task.WhenAny(tasks);
    }

    private Task ParallelNoWait(IEnumerable<NotificationHandlerExecutor> handlers, INotification notification,
        CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
        {
#pragma warning disable S1481
            var task = Task.Run(() => handler.HandlerCallback(notification, cancellationToken), default);
#pragma warning restore S1481
        }

        return Task.CompletedTask;
    }

    private async Task AsyncContinueOnException(IEnumerable<NotificationHandlerExecutor> handlers,
        INotification notification, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
            try
            {
                tasks.Add(handler.HandlerCallback(notification, cancellationToken));
            }
            catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
            {
                exceptions.Add(ex);
            }

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (AggregateException ex)
        {
            exceptions.AddRange(ex.Flatten().InnerExceptions);
        }
        catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
        {
            exceptions.Add(ex);
        }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }

    private async Task SyncStopOnException(IEnumerable<NotificationHandlerExecutor> handlers,
        INotification notification, CancellationToken cancellationToken)
    {
        foreach (var handler in handlers)
            await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
    }

    private async Task SyncContinueOnException(IEnumerable<NotificationHandlerExecutor> handlers,
        INotification notification, CancellationToken cancellationToken)
    {
        var exceptions = new List<Exception>();

        foreach (var handler in handlers)
            try
            {
                await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                exceptions.AddRange(ex.Flatten().InnerExceptions);
            }
            catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException))
            {
                exceptions.Add(ex);
            }

        if (exceptions.Any()) throw new AggregateException(exceptions);
    }
}

