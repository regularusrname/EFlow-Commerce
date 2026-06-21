using Microsoft.EntityFrameworkCore;
using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Infrastructure.Persistence;

namespace Orders.API.Features.GetOrder;

public class GetOrderQueryHandler(OrderDbContext context, ILogger<GetOrderQueryHandler> logger)
    : IRequestHandler<GetOrderQuery, Result<GetOrderResponse>>
{
    public async Task<Result<GetOrderResponse>> HandleAsync(
        GetOrderQuery request,
        CancellationToken cancellation
    )
    {
        logger.LogInformation("GetOrderQueryHandler start working. OrderId: {id}", request.OrderId);
        try
        {
            var requestedId = Guid.Parse(request.OrderId);
            var orderById = await context
                .Orders.AsNoTracking().Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == requestedId, cancellation);

            if (orderById is null)
            {
                logger.LogInformation("Cannot find order with ID: {id}", requestedId);
                return Result<GetOrderResponse>.Failure(
                    new Error("GetOrder.Failure", "Order with given ID was not found.")
                );
            }

            var responseDto = new GetOrderResponse(
                orderById.Id.ToString(),
                orderById.CustomerId.ToString(),
                orderById.Status.ToString(),
                PaymentFailedReason: null,
                orderById.CreatedAtUtc,
                orderById.Items,
                orderById.TotalPrice
            );

            logger.LogInformation("GetOrderQueryHandler returning order with ID: {id}", responseDto.OrderId);
            return Result<GetOrderResponse>.Success(responseDto);
        }
        catch (Exception ex) 
        {
            logger.LogWarning("Exception: {ex}", ex.Message);
            return Result<GetOrderResponse>.Failure(
                    new Error("GetProduct.Failure", "Unexpected error was occurred while getting the product by Id")
            );
        }
    }
}
