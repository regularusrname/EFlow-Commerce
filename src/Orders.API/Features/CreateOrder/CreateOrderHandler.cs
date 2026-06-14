using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Domain.Orders;
using Orders.API.Infrastructure.Catalog;
using Orders.API.Infrastructure.Persistence;

namespace Orders.API.Features.CreateOrder;

public class CreateOrderHandler(OrderDbContext context, ICatalogClient<Result<CatalogProductResponse>> client)
    : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly OrderDbContext _context = context;

    public async Task<Result<CreateOrderResponse>> HandleAsync(
        CreateOrderCommand request,
        CancellationToken cancellation
    )
    {
        try
        {
            List<OrderItem> domainItems = [];
            foreach (var item in request.Items)
            {
                var catalogResponse = await client.GetProductByIdAsync(item.ProductId, cancellation);

                if (!catalogResponse.IsSuccess || catalogResponse.Value is null)
                    return Result<CreateOrderResponse>.Failure(catalogResponse.Errors);
                if (item.Quantity > catalogResponse.Value.StockQuantity)
                    return Result<CreateOrderResponse>.Failure(
                            new Error("CreateOrder.InsufficientStock", "Product does not have enough stock.")
                    );

                domainItems.Add(
                        new OrderItem(catalogResponse.Value.Id, item.Quantity, catalogResponse.Value.Price)
                );
            }
            var order = new Order(Guid.Parse(request.CustomerId), domainItems);

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var response = new CreateOrderResponse(order.Id.ToString(), order.Status.ToString());
            return Result<CreateOrderResponse>.Success(response);
        }
        catch (Exception ex) 
        {
            return Result<CreateOrderResponse>.Failure(
                    new Error("CreateProduct.Failure", "Unexpected error was occurred while creating the product")
            );
        }
    }
}
