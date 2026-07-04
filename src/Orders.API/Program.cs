using System.Reflection;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Common.Endpoints;
using Orders.API.Common.Pipeline;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;
using Orders.API.Infrastructure.Catalog;
using Orders.API.Infrastructure.Messaging.Consumers;
using Orders.API.Infrastructure.Messaging.Publishers;
using Orders.API.Infrastructure.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

Log.Information("Start application");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(
        (context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services);
        }
    );
    builder.Services.AddSerilog();
    var connectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStr))
        throw new InvalidOperationException("Cannot get DB-connection string from configuration");
    builder.Services.AddDbContext<OrderDbContext>(opts =>
    {
        opts.UseNpgsql(connectionStr);
    });

    builder.Services.AddProblemDetails(opts =>
    {
        opts.CustomizeProblemDetails = ctx =>
        {
            if (ctx.Exception is BadHttpRequestException)
            {
                ctx.ProblemDetails.Status = 400;
                ctx.ProblemDetails.Title = "Ivalid request format";
                ctx.ProblemDetails.Detail = "One or more fields have invalid format";
                ctx.HttpContext.Response.StatusCode = 400;
            }
        };
    });

    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    builder.Services.AddHttpClient(
        "Catalog.API",
        httpClient =>
        {
            var baseUrl = builder.Configuration.GetValue<string>(
                "ExternalServices:Catalog:BaseUrl"
            );
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Cannot get base url for Catalog.API service");
            httpClient.BaseAddress = new Uri(baseUrl);
        }
    );
    builder.Services.AddTransient<
        ICatalogClient<Result<CatalogProductResponse>>,
        HttpCatalogClient
    >();

    builder.Services.AddDecoratedHandler<
        CreateOrderCommand,
        Result<CreateOrderResponse>,
        CreateOrderHandler
    >();
    builder.Services.AddDecoratedHandler<
        GetOrderQuery,
        Result<GetOrderResponse>,
        GetOrderQueryHandler
    >();
    builder.Services.AddDecoratedHandler<
        GetOrderQuery,
        Result<GetOrderResponse>,
        GetOrderQueryHandler
    >();

    builder.Services.AddScoped<IOrderEventPublisher, MassTransitOrderEventPublisher>();

    builder.Services.AddMassTransit(busRegConfigurator =>
    {
        busRegConfigurator.AddConsumer<PaymentSucceededConsumer>();
        busRegConfigurator.AddConsumer<PaymentFailedConsumer>();

        busRegConfigurator.UsingRabbitMq(
            (context, configurator) =>
            {
                var host = builder.Configuration.GetValue<string>(
                    "ExternalServices:MessageBroker:Host"
                )!;
                configurator.Host(
                    host,
                    "/",
                    h =>
                    {
                        h.Username(
                            builder.Configuration.GetValue<string>(
                                "ExternalServices:MessageBroker:User"
                            )!
                        );
                        h.Password(
                            builder.Configuration.GetValue<string>(
                                "ExternalServices:MessageBroker:Password"
                            )!
                        );
                    }
                );

                configurator.ConfigureEndpoints(context);
            }
        );
    });

    builder.Services.AddOpenApi();

    var app = builder.Build();

    await app.ApplyMigrationAsync();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUI(opts =>
        {
            opts.SwaggerEndpoint("/openapi/v1.json", "Dev-Demo API");
        });
    }

    app.MapOrderEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex.Message, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
// public partial class Program; //ASP0027: Unnecessary public Program class declaration
