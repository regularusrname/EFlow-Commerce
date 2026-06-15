using Catalog.API.Common;
using Catalog.API.Common.Abstractions;
using Catalog.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Features.GetProduct;

public class GetProductHandler(CatalogDbContext context)
    : IRequestHandler<GetProductQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> HandleAsync(
        GetProductQuery request,
        CancellationToken token
    )
    {
        try
        {
            var requestedId = Guid.Parse(request.ProductId);

            var response = await context.Products.FirstOrDefaultAsync(p => p.Id == requestedId);

            return response is null
                ? Result<ProductResponse>.Failure(
                    new Error("GetProduct.Failure", "Product with this ProductId not found")
                )
                : Result<ProductResponse>.Success(
                    new ProductResponse(
                        response.Id.ToString(),
                        response.Name,
                        response.Description ?? "",
                        response.Price,
                        response.StockQuantity
                    )
                );
        }
        catch (Exception ex)
        {
            return Result<ProductResponse>.Failure(
                new Error("GetProduct.Failure", "Product with this ProductId not found")
            );
        }
    }
}
