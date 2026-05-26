
public class OrderItem
{
    private OrderItem() 
    {
        Id = Guid.CreateVersion7();
    }

    public OrderItem(Guid productId, uint quantity, decimal unitPrice)
    {
        if (quantity == 0)
            throw new ArgumentException("Quantity of OrderItem should be greater then 0");
        if (unitPrice <= 0)
            throw new ArgumentException("UnitPrice of OrderItem should be greater then 0");
        if (ProductId == Guid.Empty)
            throw new ArgumentException("ProductId of OrderItem must have an Id");

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; init; }
    public Guid ProductId { get; init; } 
    public uint Quantity { get; set; } 
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
