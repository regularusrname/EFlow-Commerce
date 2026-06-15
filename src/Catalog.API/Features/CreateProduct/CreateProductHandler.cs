using Catalog.API.Common;
using Catalog.API.Common.Abstractions;
using Catalog.API.Domain.Products;
using Catalog.API.Infrastructure.Persistence;

namespace Catalog.API.Features.CreateProduct;

public class CreateProductHandler(CatalogDbContext context, ILogger<CreateProductHandler> logger)
    : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    public async Task<Result<CreateProductResponse>> HandleAsync(
        CreateProductCommand request,
        CancellationToken token
    )
    {
        logger.LogInformation("CreateProductHandler start working");
        try
        {
            var domainProduct = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity
            );

            logger.LogInformation("Saving the created product. ProductId: {id}", domainProduct.Id);
            await context.Products.AddAsync(domainProduct, token);
            await context.SaveChangesAsync();

            logger.LogInformation(
                "CreateProductHandler returning created product with ProductId: {id}",
                domainProduct.Id
            );
            return Result<CreateProductResponse>.Success(new(domainProduct.Id.ToString()));
        }
        catch (Exception ex)
        {
            logger.LogWarning("Exception: {ex}", ex.Message);
            return Result<CreateProductResponse>.Failure(
                new Error("CreateProduct.Failure", "Cannot create a product from given data")
            );
        }
    }
}
