using FluentValidation;
using Orders.API.Domain.Orders;

namespace Orders.API.Features.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty().WithMessage("CustomerId should be exists");
        RuleFor(command => command.Items).NotEmpty().WithMessage("List of OrderItems should be exists");
        RuleForEach(command => command.Items).SetValidator(new OrderItemValidator());
    }
}

public class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator()
    {

        RuleFor(i => i.ProductId).NotEmpty().WithMessage("ProductId should be exists");
        RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity should be greater than 0");
        RuleFor(i => i.UnitPrice).GreaterThan(0).WithMessage("UnitPrice should be greater than 0m");
    }
}
