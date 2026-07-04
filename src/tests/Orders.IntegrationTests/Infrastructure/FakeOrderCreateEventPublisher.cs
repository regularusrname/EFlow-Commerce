using Eflow.Contracts.IntegrationEvents.Orders;
using Orders.API.Domain.Orders;
using Orders.API.Infrastructure.Messaging.Publishers;

namespace Orders.IntegrationTests.Infrastructure;

public class FakeOrderCreateEventPublisher : IOrderEventPublisher
{
    public List<OrderCreatedIntegrationEvent> PublishedEvents { get; } = [];

    public Task PublishOrderCreatedAsync(Order order, CancellationToken token)
    {
        var orderEvent = new OrderCreatedIntegrationEvent(
            Id: Guid.CreateVersion7(),
            OrderId: order.Id,
            CustomerId: order.CustomerId,
            TotalPrice: order.TotalPrice,
            Items:
            [
                .. order.Items.Select(i => new OrderCreatedItem(
                    i.ProductId,
                    i.Quantity,
                    i.UnitPrice
                )),
            ],
            OccurredUtc: DateTime.UtcNow
        );
        PublishedEvents.Add(orderEvent);
        return Task.CompletedTask;
    }

    public void Reset()
    {
        PublishedEvents.Clear();
    }
}
