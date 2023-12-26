using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BuildingBlocks.Common.Model;
public abstract class EventMessageBase : INotification
{
    protected EventMessageBase()
    {
    }

    protected EventMessageBase(DateTime? expireDate)
    {
        ExpireDate = expireDate;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public DateTime? ExpireDate { get; }
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    public PublishStrategy PublishStrategy { get; set; } = PublishStrategy.ParallelNoWait;
}

public abstract class EventMessageBaseHandler<T> : INotificationHandler<T> where T : EventMessageBase, new()
{
    public Task Handle(T notification, CancellationToken cancellationToken)
    {
        return HandleCore(notification, cancellationToken);
    }

    protected abstract Task HandleCore(T notification, CancellationToken cancellationToken);
}
