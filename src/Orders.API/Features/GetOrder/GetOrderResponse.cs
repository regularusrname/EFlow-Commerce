using Orders.API.Domain.Orders;

namespace Orders.API.Features.GetOrder;

public record GetOrderResponse(
        Guid OrderId, 
        Guid CustomerId, 
        OrderStatus Status, 
        DateTime CreatedAt, 
        IReadOnlyCollection<OrderItem> Items,
        decimal TotalPrice
);
