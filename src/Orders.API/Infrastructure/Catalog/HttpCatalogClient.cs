using System.Net;
using Orders.API.Common;
using Orders.API.Common.Abstractions;

namespace Orders.API.Infrastructure.Catalog;

public class HttpCatalogClient(IHttpClientFactory factory, ILogger<HttpCatalogClient> logger)
    : ICatalogClient<Result<CatalogProductResponse>>
{
    public async Task<Result<CatalogProductResponse>> GetProductByIdAsync(
        string productId,
        CancellationToken token
    )
    {
        logger.LogInformation("Start HttpCatalogClient. ProductId: {productId}", productId);
        try
        {
            var client = factory.CreateClient("Catalog.API");

            logger.LogInformation("Sending request to external service. ProductId: {productId}", productId);
            var response = await client.GetAsync($"/{productId}");
            logger.LogInformation("External service send response: {response}", response);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Response has unsuccess status code: {statuscode}",
                    response.StatusCode
                );

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    logger.LogError(
                        "Catalog.API not found the product with given ID. ProductId: {id}",
                        productId
                    );
                    throw new InvalidOperationException("Cannot found the product with given Id");
                }
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    logger.LogError(
                        "Catalog.API unavailable: return {statuscode}. ProductId: {productId}",
                        response.StatusCode,
                        productId
                    );
                    throw new InvalidOperationException("Catalog.API unavailable");
                }

                logger.LogError(
                    "Unexpected error was occurred while getting the Product. ProductId: {id}",
                    productId
                );
                throw new InvalidOperationException(
                    "Response from Catalog.API has unsuccess status code"
                );
            }

            var productResponse =
                await response.Content.ReadFromJsonAsync<CatalogProductResponse>();
            if (productResponse is null)
            {
                logger.LogError("Cannot get data from response. ProductId: {productId}", productId);
                throw new InvalidOperationException(
                    "Cannot deserialize from json to CatalogProductResponse"
                );
            }

            logger.LogInformation(
                "Returning the product by given ProductId: {productId}", productId
            );
            return Result<CatalogProductResponse>.Success(productResponse);
        }
        catch (Exception ex)
        {
            logger.LogError("Exception: {exception}", ex.Message);

            if (ex.Message == "Cannot found the product with given Id")
            {
                return Result<CatalogProductResponse>.Failure(
                    new Error("CreateOrder.ProductNotFound", "Product with given ID was not found.")
                );
            }

            return Result<CatalogProductResponse>.Failure(
                new Error("Catalog.Unavailable", "Catalog service is currently unavailable.")
            );
        }
    }
}
