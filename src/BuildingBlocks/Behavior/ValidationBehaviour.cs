using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Common.Model;
using BuildingBlocks.Domain.Services;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behavior;
public sealed class ValidationBehaviour<TRequest, TResponse> : BehaviorBase<TRequest, TResponse>
    where TRequest : BaseEntity, IRequest<TResponse>
    where TResponse : ServiceResContextBase, new()
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    protected override async Task<TResponse> HandleCore(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                var firstError = failures.First();
                return new TResponse
                {
                    Error = new ValidationError
                    (
                        string.IsNullOrEmpty(firstError.ErrorCode) ? "400" : firstError.ErrorCode,
                        firstError.ErrorMessage,
                        firstError.PropertyName,
                        failures.Count > 1
                            ? failures.Select(s => (ValidationError)s).ToList()
                            : new List<ValidationError>())
                };
            }
        }

        return await next();
    }
}
