using FluentValidation;
using Orders.API.Common.Abstractions;

namespace Orders.API.Common.Pipeline;

public static class RequestBehaviorRegistrationExtensions
{
    public static IServiceCollection AddDecoratedHandler<TRequest, TResponse, THandler>(
            this IServiceCollection services)
        where TRequest : notnull
        where TResponse : Result
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        services.AddTransient<THandler>();
        services.AddTransient(provider => 
        {
            IRequestHandler<TRequest, TResponse> handler = provider.GetRequiredService<THandler>();
            handler = new ValidationBehaviorDecorator<TRequest, TResponse>(
                    handler,
                    provider.GetServices<IValidator<TRequest>>());
            return handler;          
        });
        return services;
    }
}
