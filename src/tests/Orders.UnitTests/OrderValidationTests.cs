using FluentValidation;
using Orders.API.Common;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetOrder;

namespace Orders.UnitTests;

public class OrderValidationTests
{
    private readonly List<CreateOrderItem> _validItems = 
    [
        new (Guid.CreateVersion7().ToString(), 1),
        new (Guid.CreateVersion7().ToString(), 2)
    ];

    [Fact]
    public async Task CreateOrderValidation_Success_WithValidData()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), _validItems);

        var response = await validator.ValidateAsync(command);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_CustomerIdIsEmptyGuid()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.Empty.ToString(), _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_CustomerIdIsWhiteSpace()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(" ", _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_CustomerIdIsEmptyString()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand("", _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_CustomerIdIsInvalidGuid()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand($"{Guid.CreateVersion7()}ab1", _validItems);

        var response = await validator.ValidateAsync(command);
        
        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of CustomerId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_ItemsListIsEmpty()
    {
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), []);

        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("List of OrderItems should be exists", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_ProductIdIsEmptyGuid()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(Guid.Empty.ToString(), 1)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_ProductIdIsWhitespace()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(" ", 1)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_ProductIdIsEmptyString()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new("", 1)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }


    [Fact]
    public async Task CreateOrderValidation_Failure_ProductIdIsInvalid()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new($"{Guid.CreateVersion7()}ab", 1)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Invalid format of ProductId", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_QuantityNotGreaterThanZero()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(Guid.CreateVersion7().ToString(), -1)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("Quantity should be greater than 0", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task CreateOrderValidation_Failure_UnitPriceNotGreaterThanZero()
    {
        List<CreateOrderItem> invalidItems = [.. _validItems, new(Guid.CreateVersion7().ToString(), 1)];
        var validator = new CreateOrderCommandValidator(new CreateOrderItemValidator());
        var command = new CreateOrderCommand(Guid.CreateVersion7().ToString(), invalidItems);
        
        var response = await validator.ValidateAsync(command);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
        Assert.Equal("UnitPrice should be greater than 0", response.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task GetOrderValidation_Success_ValidData()
    {
        var orderId = Guid.CreateVersion7().ToString();
        var validator = new GetOrderQueryValidator();
        var query = new GetOrderQuery(orderId);

        var response = await validator.ValidateAsync(query);

        Assert.True(response.IsValid);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public async Task GetOrderValidation_Failure_OrderIdIsEmptyGuid()
    {
        var orderId = Guid.Empty.ToString();
        var validator = new GetOrderQueryValidator();
        var query = new GetOrderQuery(orderId);

        var response = await validator.ValidateAsync(query);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
    }

    [Fact]
    public async Task GetOrderValidation_Failure_OrderIdIsEmptyString()
    {
        var orderId = "";
        var validator = new GetOrderQueryValidator();
        var query = new GetOrderQuery(orderId);

        var response = await validator.ValidateAsync(query);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
    }

    [Fact]
    public async Task GetOrderValidation_Failure_OrderIdIsWhiteSpace()
    {
        var orderId = " ";
        var query = new GetOrderQuery(orderId);
        var validator = new GetOrderQueryValidator();

        var response = await validator.ValidateAsync(query);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
    }

    [Fact]
    public async Task GetOrderValidation_Failure_OrderIdIsNumber()
    {
        var orderId = 111;
        var query = new GetOrderQuery(orderId.ToString());
        var validator = new GetOrderQueryValidator();

        var response = await validator.ValidateAsync(query);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
    }

    [Fact]
    public async Task GetOrderValidation_Failure_OrderIdIsObject()
    {
        var orderId = new { Name = "Object" };
        var query = new GetOrderQuery(orderId.ToString()!);
        var validator = new GetOrderQueryValidator();

        var response = await validator.ValidateAsync(query);

        Assert.False(response.IsValid);
        Assert.NotEmpty(response.Errors);
    }
}
