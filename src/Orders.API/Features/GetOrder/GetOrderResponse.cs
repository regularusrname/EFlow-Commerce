using Orders.API.Domain.Orders;

namespace Orders.API.Features.GetOrder;

public record GetOrderResponse(
        string OrderId, 
        string CustomerId, 
        string Status, 
        DateTime CreatedAt, 
        IReadOnlyCollection<OrderItem> Items,
        decimal TotalPrice
);
