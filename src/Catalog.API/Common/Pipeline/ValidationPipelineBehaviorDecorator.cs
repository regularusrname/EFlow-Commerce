using Catalog.API.Common.Abstractions;
using FluentValidation;

namespace Catalog.API.Common.Pipeline;

public class ValidationPipelineBehaviorDecorator<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> next,
        IEnumerable<IValidator<TRequest>> validators) 
    : IRequestHandler<TRequest, TResponse> where TResponse : IResult<TResponse> 
{ 
    public async Task<TResponse> HandleAsync(TRequest request, CancellationToken token) 
    {
        if (!validators.Any())
            await next.HandleAsync(request, token);
        
        var validationResults = await Task.WhenAll(validators.Select(async v => await v.ValidateAsync(request)));

        var errors = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(res => res.Errors)
            .Select(ver => new Error("Validation.Failure", ver.ErrorMessage))
            .ToList();

        return errors.Count > 0 ? TResponse.Failure(errors) : await next.HandleAsync(request, token);
    }
}
