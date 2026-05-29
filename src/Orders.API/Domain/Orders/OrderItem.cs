using System.Diagnostics.CodeAnalysis;

namespace Orders.API.Domain.Orders;

public class OrderItem
{
    private OrderItem() {}

    [SetsRequiredMembers]
    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity of OrderItem should be greater then 0");
        if (unitPrice <= 0)
            throw new ArgumentException("UnitPrice of OrderItem should be greater then 0");
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId of OrderItem must have an Id");
        
        Id = Guid.CreateVersion7();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public required Guid Id { get; init; }
    public required Guid ProductId { get; init; } 
    public required int Quantity { get; set; } 
    public required decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
