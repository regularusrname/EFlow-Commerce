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
    
    builder.Services.AddDecoratedHandler<CreateOrderCommand, Result<CreateOrderResponse>, CreateOrderHandler>();
    builder.Services.AddDecoratedHandler<GetOrderQuery, Result<GetOrderResponse>, GetOrderQueryHandler>();

    var app = builder.Build();
    
    app.MapOrderEndpoints();

    app.Run();
}
catch (Exception e)
{
    Console.WriteLine("Fatal: {e}", e.Message);
}
