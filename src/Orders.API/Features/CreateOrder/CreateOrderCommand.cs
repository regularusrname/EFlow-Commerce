using Orders.API.Domain.Orders;

namespace Orders.API.Features.CreateOrder;

public record CreateOrderCommand(
        Guid CustomerId, 
        IEnumerable<OrderItem> Items, 
        uint Quantity, 
        decimal UnitPrice);

