using Catalog.API.Common;
using Catalog.API.Common.Abstractions;
using Catalog.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Features.GetProduct;

public class GetProductHandler(CatalogDbContext context, ILogger<GetProductHandler> logger)
    : IRequestHandler<GetProductQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> HandleAsync(
        GetProductQuery request,
        CancellationToken token
    )
    {
        logger.LogInformation(
            "[ProductId: {id}]: GetProductHandler start working.",
            request.ProductId
        );
        try
        {
            var requestedId = Guid.Parse(request.ProductId);

            var response = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == requestedId);

            if (response is null)
            {
                logger.LogInformation("[ProductId: {id}]: Cannot find the Product.", request.ProductId);
                return Result<ProductResponse>.Failure(
                    new Error("GetProduct.Failure", "Product with this ProductId not found")
                );
            }

            logger.LogInformation("[ProductId: {id}]: GetProductHandler returning the Product.", request.ProductId);
            return Result<ProductResponse>.Success(
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
            logger.LogWarning("Exception: {ex}", ex.Message);
            return Result<ProductResponse>.Failure(
                new Error("GetProduct.Failure", "Product with this ProductId not found")
            );
        }
    }
}
