using Orders.API.Common;
using Orders.API.Common.Abstractions;

namespace Orders.API.Features.CreateOrder;

public static class CreateOrderEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderCommand command, 
            IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>> handler,
            CancellationToken ct) => 
        {
            var response = await handler.HandleAsync(command, ct);

            if (!response.IsSuccess)
            {
                var notFoundErr = response.Errors.Where(e => e.Code == "CreateOrder.ProductNotFound");
                if (notFoundErr.Any())
                    return Results.NotFound(response.Errors);

                return Results.BadRequest(response.Errors);
            }
            return Results.Created("/orders", response.Value);
        }); 
    }
}
