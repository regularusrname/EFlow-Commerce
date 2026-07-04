using Orders.API.Domain.Orders;

namespace Orders.API.Infrastructure.Messaging.Publishers;

public interface IOrderEventPublisher
{
    Task PublishOrderCreatedAsync(Order order, CancellationToken token);
}
