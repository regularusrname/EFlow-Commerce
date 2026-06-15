using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Common.Endpoints;
using Orders.API.Common.Pipeline;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;
using Orders.API.Infrastructure.Catalog;
using Orders.API.Infrastructure.Persistence;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
        true
    )
    .Build();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
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
    builder.Services.AddOpenApi();

    var app = builder.Build();

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
