using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Infrastructure.Catalog;

namespace Orders.IntegrationTests.Infrastructure;

public class FakeCatalogClient : ICatalogClient<Result<CatalogProductResponse>>
{
    private readonly Dictionary<string, CatalogProductResponse> _products = [];

    public bool IsUnavailable { get; set; } = false;

    public void AddProduct(CatalogProductResponse product)
    {
        _products.Add(product.Id.ToString(), product);
    }

    public void Clear()
    {
        _products.Clear();
        IsUnavailable = false;
    }

    public async Task<Result<CatalogProductResponse>> GetProductByIdAsync(
        string productId,
        CancellationToken token = default
    )
    {
        if (IsUnavailable)
            return Result<CatalogProductResponse>.Failure(
                 new Error("Catalog.Unavailable", "Catalog service is currently unavailable.")
            );

        _products.TryGetValue(productId, out var product);
        if (product is null)
            return Result<CatalogProductResponse>.Failure(
                new Error("CreateOrder.ProductNotFound", "Product with given ID was not found.")
            );
        return Result<CatalogProductResponse>.Success(product);
    }
}
