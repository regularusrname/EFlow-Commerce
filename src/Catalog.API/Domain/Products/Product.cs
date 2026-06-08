using System.Diagnostics.CodeAnalysis;

namespace Catalog.API.Domain.Products;

public class Product
{
    private Product() {}

    [SetsRequiredMembers]
    public Product(string name, string? description, decimal price, int stockQuantity) 
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name argument should exists");
        if (price <= 0)
            throw new ArgumentException("Price argument should be greater than 0");
        if (stockQuantity <= 0)
            throw new ArgumentException("StockQuantity should be greater than 0");

        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
        
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
    public required decimal Price { get; set; }
    public required int StockQuantity { get; set; }
    public required DateTime CreatedAt { get; init; }
}
