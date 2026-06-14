namespace Orders.API.Infrastructure.Catalog;

public record CatalogProductResponse(Guid Id, string Name, decimal Price, int StockQuantity);
