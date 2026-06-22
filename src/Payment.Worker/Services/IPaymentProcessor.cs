using Eflow.Contracts.IntegrationEvents.Orders;

namespace Payment.Worker.Services;

public interface IPaymentProcessor
{
    Task<PaymentResponse> ProcessAsync(OrderCreatedIntegrationEvent createdOrder, CancellationToken token);
}
