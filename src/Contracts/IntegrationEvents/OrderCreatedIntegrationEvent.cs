namespace Contracts.IntegrationEvents;

public record OrderCreatedIntegrationEvent(
        string OrderId, 
        string CustomerId, 
        decimal TotalPrice, 
        IReadOnlyCollection<OrderCreatedItem> Items,
        DateTime OccurredUtc
);

public record OrderCreatedItem(string ProductId, int Quantity, decimal UnitPrice);
