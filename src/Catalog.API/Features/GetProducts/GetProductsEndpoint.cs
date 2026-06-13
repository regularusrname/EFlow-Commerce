using Catalog.API.Common;
using Catalog.API.Common.Abstractions;

namespace Catalog.API.Features.GetProducts;

public static class GetProductsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (CancellationToken token, 
                    IRequestHandler<GetProductsQuery, Result<GetProductsResponse>> handler) => 
        {
            var query = new GetProductsQuery();
            var response = await handler.HandleAsync(query, token);

            return Results.Ok(response.Value);
        });
    }
}
