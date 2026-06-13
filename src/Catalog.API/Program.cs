using Catalog.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Text.Json;
using System.Reflection;
using Catalog.API.Common.Pipeline;
using Catalog.API.Features.CreateProduct;
using Catalog.API.Common;
using Catalog.API.Features.GetProduct;
using Catalog.API.Features;
using Catalog.API.Common.Endpoints;
using Catalog.API.Features.GetProducts;

var builder = WebApplication.CreateBuilder(args);

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
            ctx.ProblemDetails.Detail =  "One or more fields have invalid format";
            ctx.HttpContext.Response.StatusCode = 400;
        }
    };
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddDecoratedHandler<CreateProductCommand, Result<CreateProductResponse>, CreateProductHandler>();
builder.Services.AddDecoratedHandler<GetProductQuery, Result<ProductResponse>, GetProductHandler>();
builder.Services.AddDecoratedHandler<GetProductsQuery, Result<GetProductsResponse>, GetProductsHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opts =>
    {
        opts.SwaggerEndpoint("/openapi/v1.json", "Dev-Demo Catalog API");
    });
}

app.MapProductEndpoints();

app.Run();
