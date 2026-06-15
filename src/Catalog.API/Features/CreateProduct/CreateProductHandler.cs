using Catalog.API.Common;
using Catalog.API.Common.Abstractions;
using Catalog.API.Domain.Products;
using Catalog.API.Infrastructure.Persistence;

namespace Catalog.API.Features.CreateProduct;

public class CreateProductHandler(CatalogDbContext context)
    : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    public async Task<Result<CreateProductResponse>> HandleAsync(
        CreateProductCommand request,
        CancellationToken token
    )
    {
        try
        {
            var domainProduct = new Product(
                request.Name,
                request.Description,
                request.Price,
                request.StockQuantity
            );

            await context.Products.AddAsync(domainProduct, token);
            await context.SaveChangesAsync();

            return Result<CreateProductResponse>.Success(new(domainProduct.Id.ToString()));
        }
        catch (Exception ex) 
        {
            return Result<CreateProductResponse>.Failure(
                    new Error("CreateProduct.Failure", "Cannot create a product from given data")
                    );
        }
    }
}
