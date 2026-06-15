using System.Reflection;
using Catalog.API.Common;
using Catalog.API.Common.Endpoints;
using Catalog.API.Common.Pipeline;
using Catalog.API.Features;
using Catalog.API.Features.CreateProduct;
using Catalog.API.Features.GetProduct;
using Catalog.API.Features.GetProducts;
using Catalog.API.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Start application");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) => 
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    });
    builder.Services.AddSerilog();

    var connectionStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStr))
        throw new InvalidOperationException("Cannot get DB-connection string from configuration");

    builder.Services.AddDbContext<CatalogDbContext>(opts =>
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
                ctx.ProblemDetails.Title = "Invalid request format";
                ctx.ProblemDetails.Detail = "One or more fields have invalid format";
                ctx.HttpContext.Response.StatusCode = 400;
            }
        };
    });

    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    builder.Services.AddDecoratedHandler<
        CreateProductCommand,
        Result<CreateProductResponse>,
        CreateProductHandler
    >();
    builder.Services.AddDecoratedHandler<
        GetProductQuery,
        Result<ProductResponse>,
        GetProductHandler
    >();
    builder.Services.AddDecoratedHandler<
        GetProductsQuery,
        Result<GetProductsResponse>,
        GetProductsHandler
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
            opts.SwaggerEndpoint("/openapi/v1.json", "Dev-Demo Catalog API");
        });

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        await context.Database.MigrateAsync();
    }

    app.MapProductEndpoints();

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
