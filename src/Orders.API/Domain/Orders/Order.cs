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
    public string? PaymentFailedReason { get; private set; }

    public void MarkPaymentProcessing()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Status properties should have the 'Pending' value");

        Status = OrderStatus.PaymentProcessing;
    }

    public void MarkAsPaid()
    {
        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot mark as paid the order with status 'Cancelled'");

        //for milestone 3
        if (Status == OrderStatus.PaymentFailed)
            throw new InvalidOperationException(
                    "[For this stage of development]: Cannot mark as paid the order with status 'PaymentFailed'"
            );

        Status = OrderStatus.Paid;
    }

    public void MarkPaymentFailed(string reason)
    {
        if (Status == OrderStatus.Paid)
            throw new InvalidOperationException("Cannot mark payment as failed the order with status 'Paid'");

        if (Status == OrderStatus.Cancelled)
            return;
        
        PaymentFailedReason = reason;
        Status = OrderStatus.PaymentFailed;
    }

    public void Cancel()
    {
        Status = OrderStatus.Cancelled;
    }

    private decimal CalculateTotalPrice()
    {
        decimal totalPrice = 0; 
        foreach (var item in Items)
            totalPrice += item.TotalPrice;

        return totalPrice;
    }
}
