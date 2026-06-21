using Eflow.Contracts.IntegrationEvents.Payments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure.Persistence;

namespace Orders.API.Infrastructure.Messaging.Consumers;

public class PaymentSucceededConsumer(
    OrderDbContext dbContext,
    ILogger<PaymentSucceededConsumer> logger
) : IConsumer<PaymentSucceededIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentSucceededIntegrationEvent> context)
    {
        logger.LogInformation(
            "PaymentSucceededConsumer: Recieve message from broker. Id: {id}",
            context.MessageId
        );

        try
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

            if (order is null)
            {
                logger.LogWarning("PaymentSucceededConsumer: Order not found. Id: {id}", context.Message.OrderId);
                throw new InvalidOperationException("Cannot find the Order with given Id");
            }

            order.MarkAsPaid();

            await dbContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            logger.LogError("Exception: {ex}", ex.Message);
        }
    }
}
