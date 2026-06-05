using FluentValidation;

namespace Orders.API.Features.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator(CreateOrderItemValidator itemValidator)
    {
        RuleFor(command => command.CustomerId)
            .Must(id => !string.IsNullOrWhiteSpace(id) 
                        && Guid.TryParse(id, out var guid) 
                        && guid != Guid.Empty)
            .WithMessage("Invalid format of CustomerId");
        RuleFor(command => command.Items).NotEmpty().WithMessage("List of OrderItems should be exists");
        RuleFor(command => command.Items).NotNull();

        RuleForEach(command => command.Items).SetValidator(itemValidator);
    }
}

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItem>
{
    public CreateOrderItemValidator()
    {
        RuleFor(i => i.ProductId)
            .Must(id => !string.IsNullOrWhiteSpace(id) 
                        && Guid.TryParse(id, out var guid) 
                        && guid != Guid.Empty)
            .WithMessage("Invalid format of ProductId");
        RuleFor(i => i.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity should be greater than 0");
        RuleFor(i => i.UnitPrice)
            .GreaterThan(0)
            .WithMessage("UnitPrice should be greater than 0");

    }
}
