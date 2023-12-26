using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Services;
using MediatR;

namespace BuildingBlocks.Common.Model;
public abstract class BaseEntity
{
    public List<EventMessageBase> BeforeNextEvents { get; set; } = new();
    public List<EventMessageBase> AfterNextEvents { get; set; } = new();
}

public abstract class BehaviorBase<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
    where TRequest : BaseEntity, IRequest<TResult>
    where TResult : ServiceResContextBase, new()
{
    public Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        return HandleCore(request, cancellationToken, next);
    }

    protected TResult Failure(ValidationError error)
    {
        return new TResult { Error = error };
    }

    protected TResult Failure(string code, string message)
    {
        return new TResult { Error = new ValidationError(code, message) };
    }

#pragma warning disable CA1068 // CancellationToken parameters must come last
    protected abstract Task<TResult> HandleCore(TRequest request, CancellationToken cancellationToken,
#pragma warning restore CA1068 // CancellationToken parameters must come last
        RequestHandlerDelegate<TResult> next);
}
