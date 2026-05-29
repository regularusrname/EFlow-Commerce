using System.Diagnostics.CodeAnalysis;

namespace Orders.API.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> _items = [];

    private Order() {}

    [SetsRequiredMembers]
    public Order(Guid customerId, IEnumerable<OrderItem> items)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId of Order must exist");
        if (items is null || !items.Any())
            throw new ArgumentException("Items of Order must exist");

        Id = Guid.CreateVersion7();
        Status = OrderStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;

        CustomerId = customerId;
        _items = [.. items];
    }
    
    public required Guid Id { get; init; }
    public required Guid CustomerId { get; init; }
    public required OrderStatus Status { get; set; }
    public required DateTime CreatedAtUtc { get; init; }
    public IReadOnlyCollection<OrderItem> Items => _items;
    public decimal TotalPrice => CalculateTotalPrice();

    private decimal CalculateTotalPrice()
    {
        decimal totalPrice = 0; 
        foreach (var item in Items)
            totalPrice += item.TotalPrice;

        return totalPrice;
    }
}
