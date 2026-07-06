using System.Net.Http.Json;
using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orders.API.Domain.Orders;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;
using Orders.API.Infrastructure.Messaging.Consumers;
using Orders.API.Infrastructure.Persistence;
using Eflow.Contracts.IntegrationEvents.Payments;
using Orders.IntegrationTests.Infrastructure;

namespace Orders.IntegrationTests;

public class OrderApiConsumersTests(OrderApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task SuccessPayment_ChangeOrderStatus()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<OrderDbContext>(opts =>
            {
                opts.UseNpgsql(Factory.ConnectionString);
            })
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<PaymentSucceededConsumer>();
            })
            .AddLogging()
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var validProductId = Guid.CreateVersion7();
        Factory.CatalogClient.AddProduct(new(validProductId, "Product1", 34.35m, 4));
        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[] { new { productId = validProductId, quantity = 2 } },
        };

        var response = await Client.PostAsJsonAsync("/orders", request);
        var jsonResponse = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OrderStatus.PaymentProcessing.ToString(), jsonResponse!.Status);

        await harness.Bus.Publish(
            new PaymentSucceededIntegrationEvent(
                Guid.CreateVersion7(),
                Guid.Parse(jsonResponse!.OrderId),
                Guid.CreateVersion7(),
                DateTime.UtcNow
            )
        );
        
        var isConsumed = await harness.Consumed.Any<PaymentSucceededIntegrationEvent>();
        Assert.True(isConsumed);

        var getResponse = await Client.GetAsync($"/orders/{jsonResponse.OrderId}");
        var getJsonResponse = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();

        Assert.Equal(OrderStatus.Paid.ToString(), getJsonResponse!.Status);
    }

    [Fact]
    public async Task FailedPayment_FailedPaymentStatus_OrderHasReasonOfFailure()
    {
        await using var provider = new ServiceCollection()
            .AddDbContext<OrderDbContext>(opts =>
            {
                opts.UseNpgsql(Factory.ConnectionString);
            })
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<PaymentFailedConsumer>();
            })
            .AddLogging()
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var expectedReason = "CardDeclined";
        var validProductId = Guid.CreateVersion7();
        Factory.CatalogClient.AddProduct(new(validProductId, "Product1", 34.35m, 4));
        
        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[] { new { productId = validProductId, quantity = 2 } },
        };

        var response = await Client.PostAsJsonAsync("/orders", request);
        var jsonResponse = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(OrderStatus.PaymentProcessing.ToString(), jsonResponse!.Status);

        await harness.Bus.Publish(
            new PaymentFailedIntegrationEvent(
                Guid.CreateVersion7(),
                Guid.Parse(jsonResponse!.OrderId),
                expectedReason,
                DateTime.UtcNow
            )
        );
        
        var isConsumed = await harness.Consumed.Any<PaymentFailedIntegrationEvent>();
        Assert.True(isConsumed);

        var getResponse = await Client.GetAsync($"/orders/{jsonResponse.OrderId}");
        Assert.True(getResponse.IsSuccessStatusCode);

        var getJsonResponse = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>();

        Assert.Equal(OrderStatus.PaymentFailed.ToString(), getJsonResponse!.Status);
        Assert.Equal(expectedReason, getJsonResponse.PaymentFailedReason);
    }
}
