using Catalog.API.Common.Abstractions;
using FluentValidation;

namespace Catalog.API.Common.Pipeline;

public static class DecoratorPipelineRegistrationExtension
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
            handler = new ValidationPipelineBehaviorDecorator<TRequest, TResponse>(
                    handler,
                    provider.GetServices<IValidator<TRequest>>()
            );
            return handler;
        });
        return services;
    }
}
