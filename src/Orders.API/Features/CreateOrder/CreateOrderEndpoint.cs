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

            if (response.IsFailure)
                return Results.BadRequest(response.Error);
            return Results.Created("/orders", response.Value);
        }); 
    }
}
