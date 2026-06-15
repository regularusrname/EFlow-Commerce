using FluentValidation;
using Orders.API.Common.Abstractions;

namespace Orders.API.Common.Pipeline;

public class ValidationBehaviorDecorator<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> next,
        IEnumerable<IValidator<TRequest>> validators)
    : IRequestHandler<TRequest, TResponse> where TResponse : IResult<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellation)
    {
        if (!validators.Any())
        {
            return await next.HandleAsync(request, cancellation);
        }
        
        var results = await Task.WhenAll(validators.Select(async v => await v.ValidateAsync(request)));

        var errors = results
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Select(e => new Error("Validation.Failure", e.ErrorMessage))
            .ToList();


        return errors.Count > 0 ? TResponse.Failure(errors) :  await next.HandleAsync(request, cancellation);
    }
}
