using System.Reflection;
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
        if (!validators.Any())
            return await next.HandleAsync(request, cancellation);
        
        var results = await Task.WhenAll(validators.Select(async v => await v.ValidateAsync(request)));
        
        var errors = results
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .Select(e => new Error("Validation.Failure", e.ErrorMessage))
            .ToList();

        if (errors.Count > 0)
        {
            return GenerateFailure(errors);
        }

        return await next.HandleAsync(request, cancellation);
    }

    private static TResponse GenerateFailure(IReadOnlyCollection<Error> errors)
    {
        var resultTypeDef = typeof(TResponse);
        if (!resultTypeDef.IsGenericType)
            throw new InvalidOperationException("Generate result: Invalid TResponse type");

        var genericResultDef = resultTypeDef.GetGenericTypeDefinition();
        if (genericResultDef != typeof(Result<>))
            throw new InvalidOperationException("Generate result: Cannot get generic type definition from TResponse type");

        var failureStaticMethod = resultTypeDef.GetMethod(
                "Failure", 
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                types: [typeof(IReadOnlyCollection<Error>)],
                binder: null,
                modifiers: null);
        
        var result = failureStaticMethod?.Invoke(null, [errors]) 
                    ?? throw new 
                    InvalidOperationException("Generate result: Cannot invoke Failure method from TResponse type");
        
        return (TResponse)result;
    }
}
