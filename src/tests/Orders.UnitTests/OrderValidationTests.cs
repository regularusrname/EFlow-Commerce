using Orders.API.Features.CreateOrder;

namespace Orders.UnitTests;

public class OrderValidationTests
{
    private readonly List<CreateOrderItem> _validItems = 
    [
        new (Guid.CreateVersion7().ToString(), 1, 10m),
        new (Guid.CreateVersion7().ToString(), 2, 20m)
    ];

    [Fact]
    public async Task SuccessValidationResultWhenValidData()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), _validItems);

        var response = await validator.ValidateAsync(command);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async Task FailureResultWhenCustomerIdIsEmptyGuid()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.Empty.ToString(), _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenCustomerIdIsWhiteSpace()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(" ", _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenCustomerIdIsEmptyString()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand("", _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenCustomerIdIsInvalidGuid()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand($"{Guid.CreateVersion7()}ab1", _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenItemsListIsEmpty()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), []);

        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("List of OrderItems should be exists", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenOrderItemProductIdIsEmptyGuid()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(Guid.Empty.ToString(), 1, 1m)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenOrderItemProductIdIsWhitespace()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(" ", 1, 1m)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenOrderItemProductIdIsEmptyString()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new("", 1, 1m)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }


    [Fact]
    public async Task FailureResultWhenOrderItemProductIdIsInvalid()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new($"{Guid.CreateVersion7()}ab", 1, 1m)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenQuantityInCreateOrderItemNotGreaterThanZero()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(Guid.CreateVersion7().ToString(), -1, 1m)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Quantity should be greater than 0", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task FailureResultWhenUnitPriceInCreateOrderItemNotGreaterThanZero()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(Guid.CreateVersion7().ToString(), 1, -1m)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("UnitPrice should be greater than 0", response.Errors.First().ErrorMessage);
    }
}
