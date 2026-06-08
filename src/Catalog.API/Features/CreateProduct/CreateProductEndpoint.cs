namespace Catalog.API.Features.CreateProduct;

public static class CreateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", 
                async (CreateProductCommand request, CancellationToken token, CreateProductHandler handler) =>
        {
            var response = await handler.HandleAsync(request, token);

            if (!response.IsSuccess)
                return Results.BadRequest(response.Errors);

            return Results.Created("/products", response.Value);
        });
    }
}
