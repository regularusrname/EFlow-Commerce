using System.Net;
using Orders.API.Common;
using Orders.API.Common.Abstractions;

namespace Orders.API.Infrastructure.Catalog;

public class HttpCatalogClient(IHttpClientFactory factory)
    : ICatalogClient<Result<CatalogProductResponse>>
{
    public async Task<Result<CatalogProductResponse>> GetProductByIdAsync(
        string productId,
        CancellationToken token
    )
    {
        try
        {
            var client = factory.CreateClient("Catalog.API");
            var response = await client.GetAsync($"/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidOperationException("Cannot found the product with given Id");
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    throw new InvalidOperationException("Catalog.API unavailable");
                throw new InvalidOperationException(
                    "Response from Catalog.API has unsuccess status code"
                );
            }

            var productResponse =
                await response.Content.ReadFromJsonAsync<CatalogProductResponse>();
            if (productResponse is null)
                throw new InvalidOperationException(
                    "Cannot deserialize from json to CatalogProductResponse"
                );

            return Result<CatalogProductResponse>.Success(productResponse);
        }
        catch (Exception ex)
        {
            if (ex.Message == "Cannot found the product with given Id")
                return Result<CatalogProductResponse>.Failure(
                    new Error("CreateOrder.ProductNotFound", "Product with given ID was not found.")
                );

            return Result<CatalogProductResponse>.Failure(
                new Error("Catalog.Unavailable", "Catalog service is currently unavailable.")
            );
        }
    }
}
