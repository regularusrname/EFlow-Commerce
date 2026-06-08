using System.Reflection;
using Catalog.API.Common.Abstractions;
using FluentValidation;

namespace Catalog.API.Common.Pipeline;

public class ValidationPipelineBehaviorDecorator<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> next,
        IEnumerable<IValidator<TRequest>> validators) 
    : IRequestHandler<TRequest, TResponse> where TResponse : Result 
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

        return errors.Count > 0 ? GenerateFailureResult(errors) : await next.HandleAsync(request, token);
    }

    private TResponse GenerateFailureResult(List<Error> errors)
    {
        var responseTypeDef = typeof(TResponse);

        if (!responseTypeDef.IsGenericType)
            throw new InvalidOperationException("TResponse is not a generic type of Result");

        var genericResponseDef = responseTypeDef.GetGenericTypeDefinition();
        if (genericResponseDef != typeof(Result<>))
            throw new InvalidOperationException("Cannot get generic definition of Result from TResponse");
        
        var failureResultMethod = responseTypeDef.GetMethod("Failure", 
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static, 
                binder: null, 
                types: [typeof(IReadOnlyCollection<Error>)],
                modifiers: null);

        if (failureResultMethod is null)
            throw new InvalidOperationException("Cannot get Result.Failure() from TResponse");

        var result = failureResultMethod.Invoke(null, [errors]);

        return result is null 
            ? throw new InvalidOperationException("Cannot generateresult from TResponse type") 
            : (TResponse)result;
    }
}
