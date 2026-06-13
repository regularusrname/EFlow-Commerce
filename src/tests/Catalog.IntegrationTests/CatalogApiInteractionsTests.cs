using System.Net;
using System.Net.Http.Json;
using Catalog.API.Common;
using Catalog.API.Features;
using Catalog.API.Features.CreateProduct;
using Catalog.IntegrationTests.Infrastructure;

namespace Catalog.IntegrationTests;

public class CatalogApiInteractionsTests(CatalogApiFactory factory)
    : CatalogIntegrationTestBase(factory)
{
    [Fact]
    public async Task CreateProduct_Success_ValidData()
    {
        var request = new
        {
            name = "Some product",
            description = "desc for some product",
            price = 35.34,
            stockQuantity = 69,
        };

        var response = await Client.PostAsJsonAsync("/products", request);
        // Debug(response.ToString());
        Assert.True(response.IsSuccessStatusCode);
        // Debug(await response.Content.ReadAsStringAsync());
        var jsonResponse = await response.Content.ReadFromJsonAsync<CreateProductResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.NotEqual(Guid.Empty.ToString(), jsonResponse.ProductId);
    }

    [Fact]
    public async Task CreateProduct_Success_ParsingNumberFromString()
    {
        var request = new 
        {
            name = "product",
            description = "desc for product",
            price = "23.23",
            stockQuantity = 23
        };

        var response = await Client.PostAsJsonAsync("/products", request);
        // Debug(response.ToString());
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var jsonResponse = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
        Assert.NotNull(jsonResponse);

        var productResponse = await Client.GetAsync($"/products/{jsonResponse.ProductId}");
        Assert.True(productResponse.IsSuccessStatusCode);

        var product = await productResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(product);
        Assert.Equal(23.23m, product.Price);
    }


    [Fact]
    public async Task CreateProduct_Failure_EmptyBody()
    {
        var request = new { };

        var response = await Client.PostAsJsonAsync("/products", request);
        Assert.False(response.IsSuccessStatusCode);
        // Debug(await response.Content.ReadAsStringAsync());
        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.NotEmpty(jsonResponse);
        Assert.Equal("Validation.Failure", jsonResponse.First().Code);
    }

    [Fact]
    public async Task CreateProduct_Failure_InvalidFormatJson()
    {
        var request = new
        {
            product = "Some product",
            description_for_product = "desc for some product",
            price = 35.34,
            stockQuantity = 69,
            somefield = true
        };

        var response = await Client.PostAsJsonAsync("/products", request);
        // Debug(response.ToString());
        Assert.False(response.IsSuccessStatusCode);
        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();
        // Debug( await response.Content.ReadAsStringAsync());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(jsonResponse);
        Assert.NotEmpty(jsonResponse);
        Assert.Equal("Validation.Failure", jsonResponse.First().Code);
    }

    [Fact]
    public async Task CreateProduct_Failure_InvalidDataTyped()
    {
        var request = new
        {
            name = new { name = "innerobj name" },
            description = "some description",
            price = 34.34,
            stockQuantity = "three"
        };

        var response = await Client.PostAsJsonAsync("/products", request);
        // Debug(response.ToString());
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_Success_GetExistedProduct()
    {
        var createRequest = new
        {
            name = "product",
            description = "desc for product",
            price = 11.22,
            stockQuantity = 10
        };

        var createResponse = await Client.PostAsJsonAsync("/products", createRequest);
        Assert.True(createResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var jsonCreateResponse = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        Assert.NotNull(jsonCreateResponse);
        Assert.NotEqual(Guid.Empty.ToString(), jsonCreateResponse.ProductId);

        var getProductResponse = await Client.GetAsync($"/products/{jsonCreateResponse.ProductId}");
        Assert.True(getProductResponse.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, getProductResponse.StatusCode);

        var jsonGetProductResponse = await getProductResponse.Content.ReadFromJsonAsync<ProductResponse>();
        Assert.NotNull(jsonGetProductResponse);
        Assert.Equal(createRequest.name, jsonGetProductResponse.Name);
        Assert.Equal(createRequest.description, jsonGetProductResponse.Desciption);
        Assert.Equal(createRequest.stockQuantity, jsonGetProductResponse.StockQuantity);
        Assert.Equal((decimal)createRequest.price, jsonGetProductResponse.Price);
    }

    [Fact]
    public async Task GetProduct_NotFound_NotExistedId()
    {
        var response = await Client.GetAsync($"/products/{Guid.Empty}");
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        // Debug(await response.Content.ReadAsStringAsync());

        var jsonResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Error>>();
        Assert.NotNull(jsonResponse);
        Assert.NotEmpty(jsonResponse);
        Assert.Equal("Validation.Failure", jsonResponse.First().Code);
    }
}
