using System.Diagnostics.CodeAnalysis;

namespace Orders.API.Domain.Orders;

public class Order
{
    [SetsRequiredMembers]
    public Order(Guid customerId, IEnumerable<OrderItem> items)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId of Order must exist");
        if (items is null || items.Count() == 0)
            throw new ArgumentException("Items of Order must exist");

        Id = Guid.CreateVersion7();
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;

        CustomerId = customerId;
        Items = [.. items];
    }
    
    public required Guid Id { get; init; }
    public required Guid CustomerId { get; init; }
    public required OrderStatus Status { get; set; }
    public required DateTime CreatedAtUtc { get; init; }
    public required List<OrderItem> Items { get; init; }
    public decimal TotalPrice => CalculateTotalPrice();

    private decimal CalculateTotalPrice()
    {
        decimal totalPrice = 0; 
        foreach (var item in Items)
            totalPrice += item.TotalPrice;

        return totalPrice;
    }
}
