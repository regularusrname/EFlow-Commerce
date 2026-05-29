using FluentValidation;
using Orders.API.Domain.Orders;

namespace Orders.API.Features.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId).NotEmpty();
        RuleFor(command => command.Items).NotEmpty();
        RuleForEach(command => command.Items).SetValidator(new OrderItemValidator());
        RuleFor(command => command.Quantity).Must(q => q > 0);
        RuleFor(command => command.UnitPrice).GreaterThan(0);
    }
}

public class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator()
    {

        RuleFor(i => i.Id).NotEmpty();
        RuleFor(i => i.ProductId).NotEmpty();
        RuleFor(i => i.Quantity).Must(q => q > 0);
        RuleFor(i => i.UnitPrice).GreaterThan(0);
    }
}
