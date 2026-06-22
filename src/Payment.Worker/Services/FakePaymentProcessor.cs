using Eflow.Contracts.IntegrationEvents.Orders;

namespace Payment.Worker.Services;

public class FakePaymentProcessor(ILogger<FakePaymentProcessor> logger) : IPaymentProcessor
{
    private readonly Random _randomazer = new();
    private readonly string[] _reasons = [
        "Customer does not have enough funds.",
        "Payment was declined by the issuer.",
        "Payment provider is temporarily unavailable.",
        "Payment processing timed out.",
        "Payment failed fraud check.",
        "Payment method is invalid or expired."
    ];

    public async Task<PaymentResponse> ProcessAsync(
        OrderCreatedIntegrationEvent createdOrder,
        CancellationToken token
    )
    {
        logger.LogInformation(
            "Start fake payment-processing. OrderId: {id}; EventId: {eventId}",
            createdOrder.OrderId,
            createdOrder.Id
        );

        var chance = await FakePaymentProcessAsync(token);
        logger.LogDebug(
            "Simulating random result for payment-processing. Current chance: {chance}",
            chance
        );

        if (chance < 2)
        {
            logger.LogInformation(
                "Payment failure. OrderId: {orderId}; EventId: {eventId}",
                createdOrder.OrderId,
                createdOrder.Id
            );

            var reason = _reasons[_randomazer.Next(0, _reasons.Length)];
            logger.LogInformation("Payment-failure reason: {reason}", reason);
            return new PaymentResponse(null, IsSuccess: false, reason);
        }

        logger.LogInformation("FakePaymentProcessor returning successful result");
        return new PaymentResponse(Guid.CreateVersion7(), IsSuccess: true, FailureReason: null);
    }

    private async Task<int> FakePaymentProcessAsync(CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromSeconds(3), token);
        return _randomazer.Next(minValue: 0, maxValue: 10);
    }
}
