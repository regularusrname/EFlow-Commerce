using FluentValidation;
using Orders.API.Common.Abstractions;

namespace Orders.API.Common.Pipeline;

public class ValidationBehaviorDecorator<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> next,
        IEnumerable<IValidator<TRequest>> validators)
    : IRequestHandler<TRequest, TResponse> where TResponse : Result
{
    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellation)
    {
        if (validators is null)
            return await next.HandleAsync(request, cancellation);
        
        var errors = validators.Select(v => v.Validate(request))
            .Where(r => r.IsValid == false)
            .SelectMany(r => r.Errors)
            .Select(e => new Error("Validation.Failure", e.ErrorMessage))
            .ToList();

        if (errors.Count > 0)
        {
            return (TResponse)Result.Failure(errors);
        }

        return await next.HandleAsync(request, cancellation);
    }
}
