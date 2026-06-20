namespace Contracts.IntegrationEvents.Orders;

public record OrderCreatedIntegrationEvent(
        string Id,
        string OrderId, 
        string CustomerId, 
        decimal TotalPrice, 
        IReadOnlyCollection<OrderCreatedItem> Items,
        DateTime OccurredUtc
);

public record OrderCreatedItem(string ProductId, int Quantity, decimal UnitPrice);
