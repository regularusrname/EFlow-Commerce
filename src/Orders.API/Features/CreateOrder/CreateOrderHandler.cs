using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Domain.Orders;
using Orders.API.Infrastructure.Catalog;
using Orders.API.Infrastructure.Persistence;
using Orders.API.Infrastructure.Messaging.Publishers;

namespace Orders.API.Features.CreateOrder;

public class CreateOrderHandler(
    OrderDbContext context,
    ICatalogClient<Result<CatalogProductResponse>> client,
    IOrderEventPublisher eventPublisher,
    ILogger<CreateOrderHandler> logger
) : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly OrderDbContext _context = context;

    public async Task<Result<CreateOrderResponse>> HandleAsync(
        CreateOrderCommand request,
        CancellationToken cancellation
    )
    {
        logger.LogInformation("CreateOrderHandler start working.");
        try
        {
            List<OrderItem> domainItems = [];

            logger.LogInformation("Start checking given products IDs");
            foreach (var item in request.Items)
            {
                var catalogResponse = await client.GetProductByIdAsync(
                    item.ProductId,
                    cancellation
                );

                if (!catalogResponse.IsSuccess || catalogResponse.Value is null)
                {
                    logger.LogWarning(
                        "CreateOrderHandler recieve failure result from CatalogClient"
                    );
                    return Result<CreateOrderResponse>.Failure(catalogResponse.Errors);
                }
                if (item.Quantity > catalogResponse.Value.StockQuantity)
                {
                    logger.LogWarning(
                        "Quantity({q}) for creating order greater than StockQuantity({sq}) from Catalog.",
                        item.Quantity,
                        catalogResponse.Value.StockQuantity
                    );
                    return Result<CreateOrderResponse>.Failure(
                        new Error(
                            "CreateOrder.InsufficientStock",
                            "Product does not have enough stock."
                        )
                    );
                }

                domainItems.Add(
                    new OrderItem(
                        catalogResponse.Value.Id,
                        item.Quantity,
                        catalogResponse.Value.Price
                    )
                );
            }
            var order = new Order(Guid.Parse(request.CustomerId), domainItems);

            logger.LogInformation("Adding created order to Database. OrderId: {id}", order.Id);

            order.MarkPaymentProcessing();
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            await eventPublisher.PublishOrderCreatedAsync(order, cancellation);

            var response = new CreateOrderResponse(order.Id.ToString(), order.Status.ToString());

            logger.LogInformation("CreateOrderHandler return success result. OrderId: {id}", response.OrderId);
            return Result<CreateOrderResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Exception: {ex}", ex.Message);
            return Result<CreateOrderResponse>.Failure(
                new Error(
                    "CreateProduct.Failure",
                    "Unexpected error was occurred while creating the product"
                )
            );
        }
    }
}
