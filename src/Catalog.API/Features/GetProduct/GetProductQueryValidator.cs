using FluentValidation;

namespace Catalog.API.Features.GetProduct;

public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
{
    public GetProductQueryValidator()
    {
        RuleFor(q => q.ProductId)
            .Must(id => Guid.TryParse(id, out var _))
            .WithMessage("ProductId has invalid GUID format");
    }
}
