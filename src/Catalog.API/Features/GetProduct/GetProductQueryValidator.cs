using FluentValidation;

namespace Catalog.API.Features.GetProduct;

public class GetProductQueryValidator : AbstractValidator<GetProductQuery>
{
    public GetProductQueryValidator()
    {
        RuleFor(q => q.ProductId)
            .Must(id => !string.IsNullOrWhiteSpace(id) 
                    && Guid.TryParse(id, out var guid) 
                    && guid != Guid.Empty)
            .WithMessage("ProductId has invalid GUID format");
    }
}
