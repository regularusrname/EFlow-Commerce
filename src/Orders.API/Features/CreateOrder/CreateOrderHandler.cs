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
        var domainItems = request.Items.Select(coi => 
        {
            return new OrderItem(Guid.Parse(coi.ProductId), coi.Quantity, coi.UnitPrice);
        });
        var order = new Order(Guid.Parse(request.CustomerId), domainItems);
        
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var response = new CreateOrderResponse(order.Id, order.Status.ToString());
        return Result<CreateOrderResponse>.Success(response);
    }
}
