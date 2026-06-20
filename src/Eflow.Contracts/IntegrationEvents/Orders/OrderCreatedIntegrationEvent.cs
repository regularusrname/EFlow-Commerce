namespace Eflow.Contracts.IntegrationEvents.Orders;

public record OrderCreatedIntegrationEvent(
        Guid Id,
        Guid OrderId, 
        Guid CustomerId, 
        decimal TotalPrice, 
        IReadOnlyCollection<OrderCreatedItem> Items,
        DateTime OccurredUtc
);

public record OrderCreatedItem(Guid ProductId, int Quantity, decimal UnitPrice);
