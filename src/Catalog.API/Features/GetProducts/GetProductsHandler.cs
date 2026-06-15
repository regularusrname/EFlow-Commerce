using Catalog.API.Common;
using Catalog.API.Common.Abstractions;
using Catalog.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Features.GetProducts;

public class GetProductsHandler(CatalogDbContext context)
    : IRequestHandler<GetProductsQuery, Result<GetProductsResponse>>
{
    public async Task<Result<GetProductsResponse>> HandleAsync(GetProductsQuery request, CancellationToken token)
    {
        try
        {
        var items = await context.Products.Select(p => 
                new ProductResponse(p.Id.ToString(),
                    p.Name,
                    p.Description ?? "",
                    p.Price,
                    p.StockQuantity))
            .ToListAsync(token) ?? [];

        return Result<GetProductsResponse>.Success(new GetProductsResponse(items));
        } catch (Exception ex)
        {
            return Result<GetProductsResponse>.Failure(
                    new Error("GetProducts.Unavailable", "Service Catalog.API curretly unavailable")
            );
        }
    }
}
