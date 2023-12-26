using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using MediatR;

namespace BuildingBlocks.Behavior;
public sealed class EventsBehaviour<TRequest, TResponse> : BehaviorBase<TRequest, TResponse>
    where TRequest : BaseEntity, IRequest<TResponse>
    where TResponse : ServiceResContextBase, new()
{
    private readonly ICustomPublisher _publisher;

    public EventsBehaviour(ICustomPublisher publisher)
    {
        _publisher = publisher;
    }

    protected override async Task<TResponse> HandleCore(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (request.BeforeNextEvents?.Any() ?? false)
            foreach (var nextEvent in request.BeforeNextEvents)
                await _publisher.Publish(nextEvent, nextEvent.PublishStrategy, default);

        try
        {
            return await next();
        }
        finally
        {
            if (request.AfterNextEvents?.Any() ?? false)
                foreach (var nextEvent in request.AfterNextEvents)
                    await _publisher.Publish(nextEvent, nextEvent.PublishStrategy, default);
        }
    }
}
