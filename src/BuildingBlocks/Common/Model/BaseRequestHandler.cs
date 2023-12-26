using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Services;
using MediatR;

namespace BuildingBlocks.Common.Model;

public abstract class BaseRequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult>
    where TRequest : BaseEntity, IRequest<TResult>
    where TResult : ServiceResContextBase, new()
{
    public Task<TResult> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return HandleCore(request, cancellationToken);
    }

    protected abstract Task<TResult> HandleCore(TRequest request, CancellationToken cancellationToken);

    protected TResult Failure(ValidationError error)
    {
        return new TResult { Error = error };
    }

    protected TResult Failure(string code, string message)
    {
        return new TResult { Error = new ValidationError(code, message) };
    }
}
