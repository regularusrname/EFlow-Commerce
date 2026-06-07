using Orders.API.Common;
using Orders.API.Common.Abstractions;

namespace Orders.API.Features.GetOrder;

public static class GetOrderEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{id:guid}", async (
            Guid id,
            IRequestHandler<GetOrderQuery, Result<GetOrderResponse>> handler,
            CancellationToken ct) => 
        {
            var query = new GetOrderQuery(id);
            var result = await handler.HandleAsync(query, ct);

            if (!result.IsSuccess)
                return Results.NotFound(result.Error);

            return Results.Ok(result.Value);
        });
    }
}
