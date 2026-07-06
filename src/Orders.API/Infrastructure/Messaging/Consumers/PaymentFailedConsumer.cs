using Eflow.Contracts.IntegrationEvents.Payments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure.Persistence;

namespace Orders.API.Infrastructure.Messaging.Consumers;

public class PaymentFailedConsumer(OrderDbContext dbContext, ILogger<PaymentFailedConsumer> logger)
    : IConsumer<PaymentFailedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
    {
        logger.LogInformation(
            "PaymentSucceededConsumer: Recieve message from broker. Id: {id}",
            context.MessageId
        );

        var order = await dbContext.Orders.FirstOrDefaultAsync(o =>
            o.Id == context.Message.OrderId
        );

        if (order is null)
        {
            logger.LogWarning(
                "PaymentSucceededConsumer: Order not found. Id: {id}",
                context.Message.OrderId
            );
            throw new InvalidOperationException("Cannot find the Order with given Id");
        }

        order.MarkPaymentFailed(context.Message.Reason);
        await dbContext.SaveChangesAsync();
    }
}
