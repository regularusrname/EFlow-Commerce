using System.Net;
using System.Net.Http.Json;
using Orders.API.Common;
using Orders.API.Domain.Orders;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;

namespace Orders.IntegrationTests;

public class OrderApiInteractionTests(OrderApiFactory factory) : IClassFixture<OrderApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateOrder_FailureResultWithInvalidRequest()
    {
        var invalidRequest = new { };

        var response = await _client.PostAsJsonAsync("/orders", invalidRequest);
        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.Equal(new Error("Validation.Failure", "Invalid format of CustomerId"), jsonResponse.First());
        // Debug(jsonResponse);

        await factory.ResetDatabaseAsync();
    }

    [Fact]
    public async Task CreateOrder_SuccessResultWithValidRequest()
    {
        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[]
            {
                new
                {
                    productId = Guid.CreateVersion7(),
                    quantity = 2,
                    unitPrice = 420.69m,
                },
            },
        };

        var response = await _client.PostAsJsonAsync("/orders", request);
        var jsonResponse = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.Equal(OrderStatus.Pending.ToString(), jsonResponse.Status);
        // Debug(await response.Content.ReadAsStringAsync());

        await factory.ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetOrderById_SuccessWhenValidId()
    {
        var expectedCustomerId = Guid.CreateVersion7();
        var expectedProductId = Guid.CreateVersion7();
        var request = new
        {
            customerId = expectedCustomerId,
            items = new[]
            {
                new
                {
                    productId = expectedProductId,
                    quantity = 2,
                    unitPrice = 420.69m,
                },
            },
        };

        var createResponse = await _client.PostAsJsonAsync("/orders", request);
        var jsonCreateResponse = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        Assert.NotNull(jsonCreateResponse);

        var newClient = factory.CreateClient();
        var getByIdResponse = await newClient.GetAsync($"/orders/{jsonCreateResponse.OrderId}");
        var getByIdJsonResponse = await getByIdResponse.Content.ReadFromJsonAsync<GetOrderResponse>();

        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);
        Assert.NotNull(getByIdJsonResponse);
        Assert.Equal(jsonCreateResponse.OrderId, getByIdJsonResponse.OrderId);
        Assert.Equal(expectedCustomerId, getByIdJsonResponse.CustomerId);
        // Debug(await getByIdResponse.Content.ReadAsStringAsync());
        Assert.NotEmpty(getByIdJsonResponse.Items);
        Assert.Equal(expectedProductId, getByIdJsonResponse.Items.First().ProductId);
        Assert.Equal(2*420.69m, getByIdJsonResponse.TotalPrice);

        await factory.ResetDatabaseAsync();
    }

    [Fact]
    public async Task GetOrderById_NotFoundWithNotExistedId()
    {
        var expectedError = new Error("GetOrder.Failure", "Order with given ID was not found.");

        var getByIdResponse = await _client.GetAsync($"/orders/{Guid.CreateVersion7()}");
        var getByIdJsonResponse = await getByIdResponse.Content.ReadFromJsonAsync<IEnumerable<Error>>();

        Assert.Equal(HttpStatusCode.NotFound, getByIdResponse.StatusCode);
        // Debug(await getByIdResponse.Content.ReadAsStringAsync());
        Assert.NotNull(getByIdJsonResponse);
        Assert.NotEmpty(getByIdJsonResponse);
        Assert.Equal(expectedError, getByIdJsonResponse.First());

        await factory.ResetDatabaseAsync();
    }

    private void Debug<T>(T value)
    {
        Console.WriteLine($"[[DEBUG]] {value}");
    }
}
