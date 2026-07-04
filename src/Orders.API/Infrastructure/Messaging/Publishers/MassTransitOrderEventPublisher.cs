using Eflow.Contracts.IntegrationEvents.Orders;
using MassTransit;
using Orders.API.Domain.Orders;

namespace Orders.API.Infrastructure.Messaging.Publishers;

public class MassTransitOrderEventPublisher(
    IPublishEndpoint publisher,
    ILogger<MassTransitOrderEventPublisher> logger
) : IOrderEventPublisher
{
    public async Task PublishOrderCreatedAsync(Order order, CancellationToken token)
    {
        logger.LogInformation("Publisher start working");
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

        logger.LogInformation("Start publishing the event: {event}", orderEvent);
        await publisher.Publish(orderEvent, token);
    }
}
