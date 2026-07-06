using Eflow.Contracts.IntegrationEvents.Orders;
using Eflow.Contracts.IntegrationEvents.Payments;
using MassTransit;
using Payment.Worker.Services;

namespace Payment.Worker.Consumers;

public class OrderCreatedConsumer(
    IPaymentProcessor processor,
    ILogger<OrderCreatedConsumer> logger
) : IConsumer<OrderCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
    {
        logger.LogInformation(
            "OrderCreatedConsumer: receive message from broker. Id: {id}",
            context.Message.Id
        );

        if (
            await processor.ProcessAsync(context.Message, context.CancellationToken)
            is PaymentResponse paymentProcessingResponse
        )
        {
            if (!paymentProcessingResponse.IsSuccess)
            {
                logger.LogInformation("Receive failed result from payment-processor");
                await context.Publish<PaymentFailedIntegrationEvent>(
                    new(
                        Guid.CreateVersion7(),
                        context.Message.OrderId,
                        paymentProcessingResponse.FailureReason!,
                        DateTime.UtcNow
                    )
                );
                return;
            }

            logger.LogInformation("Payment-processor return successful result");
            await context.Publish<PaymentSucceededIntegrationEvent>(
                new(
                    Guid.CreateVersion7(),
                    context.Message.OrderId,
                    (Guid)paymentProcessingResponse.PaymentId!,
                    DateTime.UtcNow
                )
            );
            return;
        }

        logger.LogError("OrderCreatedConsumer did not recieve response from payment-processor");
        throw new InvalidOperationException("IPaymentProcessor did not return any result");
    }
}
