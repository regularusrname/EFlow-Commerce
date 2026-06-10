using FluentValidation;

namespace Orders.API.Features.GetOrder;

public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator()
    {
        RuleFor(q => q.OrderId)
            .Must(id => !string.IsNullOrWhiteSpace(id) 
                    && Guid.TryParse(id, out var guid) 
                    && guid != Guid.Empty)
            .WithMessage("OrderId has invalid GUID format");
    }

}
