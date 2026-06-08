using Microsoft.EntityFrameworkCore;
using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Infrastructure.Persistence;

namespace Orders.API.Features.GetOrder;

public class GetOrderQueryHandler(OrderDbContext context) : IRequestHandler<GetOrderQuery, Result<GetOrderResponse>>
{
    public async Task<Result<GetOrderResponse>> HandleAsync(GetOrderQuery request, CancellationToken cancellation)
    {
        var requestedId = Guid.Parse(request.OrderId);
        var orderById = await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == requestedId, cancellation);

        if (orderById is null)
            return Result<GetOrderResponse>.Failure(new Error("GetOrder.Failure", "Order with given ID was not found."));

        var responseDto = new GetOrderResponse(
                orderById.Id.ToString(),
                orderById.CustomerId.ToString(), 
                orderById.Status.ToString(), 
                orderById.CreatedAtUtc, 
                orderById.Items, 
                orderById.TotalPrice
        );

        return Result<GetOrderResponse>.Success(responseDto);
    }
}
