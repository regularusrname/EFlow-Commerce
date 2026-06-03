using Orders.API.Domain.Orders;
using Orders.API.Features.CreateOrder;

namespace Orders.UnitTests;

public class OrderValidationTests
{
    private readonly List<OrderItem> _validItems = 
    [
        new (Guid.CreateVersion7(), 1, 10m),
        new (Guid.CreateVersion7(), 2, 20m)
    ];

    [Fact]
    public async Task SuccessValidationResultWhenValidData()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(Guid.CreateVersion7(), _validItems);

        var response = await validator.ValidateAsync(command);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async Task FailureResultWhenCustomerIdIsEmpty()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(Guid.Empty, _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("CustomerId should be exists", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureWhenItemsListIsEmpty()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(Guid.CreateVersion7(), []);

        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("List of OrderItems should be exists", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureWhenOrderItemHasNoProductId()
    {
        var validator = new CreateOrderCommandValidator();
        var command = new CreateOrderCommand(Guid.Empty, [.. _validItems, new OrderItem(Guid.Empty, 1, 1m)]);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("ProductId should be exists", response.Errors.First().ErrorMessage);
    }
}
