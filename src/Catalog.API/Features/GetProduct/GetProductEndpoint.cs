namespace Catalog.API.Features.GetProduct;

public static class GetProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id}", async (string id, CancellationToken token, GetProductHandler handler) =>
        {
            var query = new GetProductQuery(id);

            var response = await handler.HandleAsync(query, token);
            if (!response.IsSuccess)
                return Results.NotFound(response.Errors);
            return Results.Ok(response.Value);
        });
    }
}
