namespace Catalog.API.Features.GetProducts;

public record GetProductsResponse(IEnumerable<ProductResponse> Items);
