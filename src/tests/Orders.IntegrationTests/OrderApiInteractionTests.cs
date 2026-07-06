using System.Net;
using System.Net.Http.Json;
using Orders.API.Common;
using Orders.API.Domain.Orders;
using Orders.API.Features.GetOrder;
using Orders.API.Features.CreateOrder;
using Orders.IntegrationTests.Infrastructure;

namespace Orders.IntegrationTests;

public class OrderApiInteractionTests(OrderApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateOrder_FailureResultWithInvalidRequest()
    {
        var invalidRequest = new { };

        var response = await Client.PostAsJsonAsync("/orders", invalidRequest);
        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.Equal(
            new Error("Validation.Failure", "Invalid format of CustomerId"),
            jsonResponse.First()
        );
        Assert.Empty(Factory.Publisher.PublishedEvents);
    }

    [Fact]
    public async Task CreateOrder_SuccessResultWithValidRequest()
    {
        var validProductId = Guid.CreateVersion7();
        Factory.CatalogClient.AddProduct(new(validProductId, "Product1", 34.35m, 4));
        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[] { new { productId = validProductId, quantity = 2 } },
        };

        var response = await Client.PostAsJsonAsync("/orders", request);
        var jsonResponse = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.Equal(OrderStatus.PaymentProcessing.ToString(), jsonResponse.Status);
        Assert.NotEmpty(Factory.Publisher.PublishedEvents);
        Assert.Equal(jsonResponse.OrderId, Factory.Publisher.PublishedEvents[0].OrderId.ToString());
        Assert.Equal(request.customerId, Factory.Publisher.PublishedEvents[0].CustomerId);
    }

    [Fact]
    public async Task CreateOrder_Failure_ProductIdDoesNotExists()
    {
        var expectedError = new Error(
            "CreateOrder.ProductNotFound",
            "Product with given ID was not found."
        );
        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[] { new { productId = Guid.CreateVersion7(), quantity = 3 } },
        };

        var response = await Client.PostAsJsonAsync("/orders", request);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        Debug(await response.Content.ReadAsStringAsync());
        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();
        Assert.NotNull(jsonResponse);
        Assert.NotEmpty(jsonResponse);

        Assert.Equal(expectedError, jsonResponse.First());
        Assert.Empty(Factory.Publisher.PublishedEvents);
    }

    [Fact]
    public async Task CreateOrder_Failure_StockIsInsufficient()
    {
        var expectedProductId = Guid.CreateVersion7();

        Factory.CatalogClient.AddProduct(new(expectedProductId, "Product0", 34.34m, 2));

        var expectedError = new Error(
            "CreateOrder.InsufficientStock",
            "Product does not have enough stock."
        );
        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[] { new { productId = expectedProductId, quantity = 3 } },
        };

        var response = await Client.PostAsJsonAsync("/orders", request);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();
        Assert.NotNull(jsonResponse);
        Assert.NotEmpty(jsonResponse);

        Assert.Equal(expectedError, jsonResponse.First());
        Assert.Empty(Factory.Publisher.PublishedEvents);
    }

    [Fact]
    public async Task CreateOrder_Failure_Unavailable()
    {
        Factory.CatalogClient.IsUnavailable = true;
        var expectedError = new Error(
            "Catalog.Unavailable",
            "Catalog service is currently unavailable."
        );

        var request = new
        {
            customerId = Guid.CreateVersion7(),
            items = new[] { new { productId = Guid.CreateVersion7(), quantity = 3 } },
        };

        var response = await Client.PostAsJsonAsync("/orders", request);
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();
        Assert.NotNull(jsonResponse);
        Assert.NotEmpty(jsonResponse);

        Assert.Equal(expectedError, jsonResponse.First());
        Assert.Empty(Factory.Publisher.PublishedEvents);
    }

    [Fact]
    public async Task GetOrderById_SuccessWhenValidId()
    {
        var expectedCustomerId = Guid.CreateVersion7();
        var expectedProductId = Guid.CreateVersion7();
        Factory.CatalogClient.AddProduct(new(expectedProductId, "Product1", 420.69m, 3));
        var request = new
        {
            customerId = expectedCustomerId.ToString(),
            items = new[] { new { productId = expectedProductId.ToString(), quantity = 2 } },
        };

        var createResponse = await Client.PostAsJsonAsync("/orders", request);
        var jsonCreateResponse =
            await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>();
        Assert.NotNull(jsonCreateResponse);
        Assert.NotEmpty(Factory.Publisher.PublishedEvents);

        var newClient = Factory.CreateClient();
        var getByIdResponse = await newClient.GetAsync($"/orders/{jsonCreateResponse.OrderId}");
        var getByIdJsonResponse =
            await getByIdResponse.Content.ReadFromJsonAsync<GetOrderResponse>();

        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);
        Assert.NotNull(getByIdJsonResponse);
        Assert.Equal(jsonCreateResponse.OrderId, getByIdJsonResponse.OrderId.ToString());
        Assert.Equal(expectedCustomerId, Guid.Parse(getByIdJsonResponse.CustomerId));
        Assert.NotEmpty(getByIdJsonResponse.Items);
        Assert.Equal(expectedProductId, getByIdJsonResponse.Items.First().ProductId);
        Assert.Equal(2 * 420.69m, getByIdJsonResponse.TotalPrice);
        
        Assert.Equal(getByIdJsonResponse.OrderId, Factory.Publisher.PublishedEvents[0].OrderId.ToString());
        Assert.Equal(getByIdJsonResponse.CustomerId, Factory.Publisher.PublishedEvents[0].CustomerId.ToString());
        Assert.Equal(getByIdJsonResponse.TotalPrice, Factory.Publisher.PublishedEvents[0].TotalPrice);
    }

    [Fact]
    public async Task GetOrderById_NotFoundWithNotExistedId()
    {
        var expectedError = new Error("GetOrder.Failure", "Order with given ID was not found.");

        var getByIdResponse = await Client.GetAsync($"/orders/{Guid.CreateVersion7()}");
        var getByIdJsonResponse = await getByIdResponse.Content.ReadFromJsonAsync<
            IEnumerable<Error>
        >();

        Assert.Equal(HttpStatusCode.NotFound, getByIdResponse.StatusCode);
        Assert.NotNull(getByIdJsonResponse);
        Assert.NotEmpty(getByIdJsonResponse);
        Assert.Equal(expectedError, getByIdJsonResponse.First());
    }
}
