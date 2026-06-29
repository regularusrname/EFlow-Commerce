using Eflow.Contracts.IntegrationEvents.Orders;
using Eflow.Contracts.IntegrationEvents.Payments;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Payment.Worker.Consumers;
using Payment.Worker.Services;

namespace Payment.Worker.UnitTests;

public class PaymentWorkerTests
{
    [Fact]
    public async Task OrderCreatedConsumer_PublishPaymentSucceededIntegrationEvent()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<OrderCreatedConsumer>();
            })
            .AddScoped<IPaymentProcessor, SuccessPaymentProcessor>()
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var orderCreateEvent = new OrderCreatedIntegrationEvent(
            Id: Guid.CreateVersion7(),
            OrderId: Guid.CreateVersion7(),
            CustomerId: Guid.CreateVersion7(),
            TotalPrice: 100m,
            Items: [new(Guid.CreateVersion7(), 3, 50m)],
            OccurredUtc: DateTime.UtcNow
        );

        await harness.Bus.Publish(orderCreateEvent);

        Assert.True(await harness.Consumed.Any<OrderCreatedIntegrationEvent>());
        Assert.True(await harness.Published.Any<PaymentSucceededIntegrationEvent>());
        Assert.Equal(
            orderCreateEvent.OrderId,
            (
                await harness
                    .Published.SelectAsync<PaymentSucceededIntegrationEvent>()
                    .FirstOrDefaultAsync()
            )
                ?.Context
                .Message
                .OrderId
        );
        Assert.False(await harness.Published.Any<PaymentFailedIntegrationEvent>());
    }

    [Fact]
    public async Task OrderCreatedConsumer_PublishPaymentFailedIntegrationEvent()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<OrderCreatedConsumer>();
            })
            .AddScoped<IPaymentProcessor, FailPaymentProcessor>()
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var createOrderEvent = new OrderCreatedIntegrationEvent(
            Id: Guid.CreateVersion7(),
            OrderId: Guid.CreateVersion7(),
            CustomerId: Guid.CreateVersion7(),
            TotalPrice: 100m,
            Items: [new(Guid.CreateVersion7(), 3, 50m)],
            OccurredUtc: DateTime.UtcNow
        );

        await harness.Bus.Publish(createOrderEvent);

        Assert.True(await harness.Consumed.Any<OrderCreatedIntegrationEvent>());
        Assert.False(await harness.Published.Any<PaymentSucceededIntegrationEvent>());
        Assert.True(await harness.Published.Any<PaymentFailedIntegrationEvent>());
        Assert.NotNull(
            (
                await harness
                    .Published.SelectAsync<PaymentFailedIntegrationEvent>()
                    .FirstOrDefaultAsync()
            )
                ?.Context
                .Message
                .Reason
        );
    }

    private class SuccessPaymentProcessor : IPaymentProcessor
    {
        public Task<PaymentResponse> ProcessAsync(
            OrderCreatedIntegrationEvent createdOrder,
            CancellationToken token
        )
        {
            return Task.FromResult(new PaymentResponse(Guid.CreateVersion7(), true, null));
        }
    }

    private class FailPaymentProcessor : IPaymentProcessor
    {
        private readonly string[] _reasons =
        [
            "Customer does not have enough funds.",
            "Payment was declined by the issuer.",
            "Payment provider is temporarily unavailable.",
            "Payment processing timed out.",
            "Payment failed fraud check.",
            "Payment method is invalid or expired.",
        ];

        public Task<PaymentResponse> ProcessAsync(
            OrderCreatedIntegrationEvent createdOrder,
            CancellationToken token
        )
        {
            return Task.FromResult(new PaymentResponse(null, false, GetRandomReason()));
        }

        private string GetRandomReason()
        {
            var rand = new Random();

            return _reasons[rand.Next(0, _reasons.Length)];
        }
    }
}
