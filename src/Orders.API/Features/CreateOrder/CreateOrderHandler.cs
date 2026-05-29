using Orders.API.Common;
using Orders.API.Common.Abstractions;
using Orders.API.Domain.Orders;
using Orders.API.Infrastructure.Persistence;

namespace Orders.API.Features.CreateOrder;

public class CreateOrderHandler(OrderDbContext context) : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly OrderDbContext _context = context;

    public async Task<Result<CreateOrderResponse>> HandleAsync(CreateOrderCommand request, CancellationToken cancellation)
    {
        var order = new Order(request.CustomerId, request.Items);
        
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var response = new CreateOrderResponse(order.Id, order.Status);
        return Result<CreateOrderResponse>.Success(response);
    }
}
