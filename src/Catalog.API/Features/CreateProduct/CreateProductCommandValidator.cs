using FluentValidation;

namespace Catalog.API.Features.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("The Product should be named")
            .Length(min: 3, 64)
            .WithMessage("Invalid length of Name");

        RuleFor(c => c.Description)
            .MaximumLength(256)
            .WithMessage("Description is too large");

        RuleFor(c => c.Price)
            .GreaterThan(0)
            .WithMessage("Price should be greater than 0");

        RuleFor(c => c.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("StockQuantity cannot be less than 0");
    }
}
