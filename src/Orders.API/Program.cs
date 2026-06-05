using System.Reflection;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Orders.API.Common;
using Orders.API.Common.Endpoints;
using Orders.API.Common.Pipeline;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;
using Orders.API.Infrastructure.Persistence;

try
{
    var builder = WebApplication.CreateBuilder(args);

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
            if (ctx.Exception is FormatException or JsonException)
            {
                ctx.ProblemDetails.Status = 400;
                ctx.ProblemDetails.Title = "Ivalid request format";
                ctx.ProblemDetails.Detail = "One or more fields have invalid format";
                ctx.HttpContext.Response.StatusCode = 400;
            }
        };
    });

    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    builder.Services.AddDecoratedHandler<CreateOrderCommand, Result<CreateOrderResponse>, CreateOrderHandler>();
    builder.Services.AddDecoratedHandler<GetOrderQuery, Result<GetOrderResponse>, GetOrderQueryHandler>();
    builder.Services.AddOpenApi();

    var app = builder.Build();
    
    app.UseExceptionHandler();

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

} catch (Exception e)
{
    Console.WriteLine("Fatal: {e}", e.Message);
}

// public partial class Program; //ASP0027: Unnecessary public Program class declaration
