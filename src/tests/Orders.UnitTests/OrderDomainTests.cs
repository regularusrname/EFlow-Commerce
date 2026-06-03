using Orders.API.Domain.Orders;

namespace Orders.UnitTests;

public class OrderDomainTests
{
    [Fact]
    public void SuccessCreateOrderWithValidData()
    {
        var customerId = Guid.CreateVersion7();
        List<OrderItem> items = [
            new(Guid.CreateVersion7(), 1, 10m),
            new(Guid.CreateVersion7(), 2, 20m),
            new(Guid.CreateVersion7(), 3, 30m)
        ];
        var expectedTotalSum = items.Select(i => i.TotalPrice).Sum();

        var order = new Order(customerId, items);

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.NotEqual(default, order.CreatedAtUtc);
        Assert.NotEmpty(order.Items);
        Assert.Equal(items, order.Items);
        Assert.NotEqual(0m, order.TotalPrice);
        Assert.Equal(expectedTotalSum, order.TotalPrice);
    }

    [Fact]
    public void ThrowExceptionWhenCreateOrderItemWithInvalidData()
    {
        List<OrderItem> validItems = [
            new(Guid.CreateVersion7(), 1, 10m),
            new(Guid.CreateVersion7(), 2, 20m),
        ];

        // creating an order without customer id should fail
        Assert.Throws<ArgumentException>(() => new Order(Guid.Empty, validItems));
        // creating an order without items should fail
        Assert.Throws<ArgumentException>(() => new Order(Guid.CreateVersion7(), []));
        // creating an order item with empty product id should fail
        Assert.Throws<ArgumentException>(() 
                => new Order(Guid.CreateVersion7(), [.. validItems, new OrderItem(Guid.Empty, 1, 1m)]));
        // creating an order item with quantity less than or equal to zero should fail
        Assert.Throws<ArgumentException>(() 
                => new Order(Guid.CreateVersion7(), [.. validItems, new OrderItem(Guid.Empty, -1, 1m)]));
        // creating an order item with price less than or equal to zero should fail
        Assert.Throws<ArgumentException>(() 
                => new Order(Guid.CreateVersion7(), [.. validItems, new OrderItem(Guid.Empty, 1, -1.67m)]));

    }
}
