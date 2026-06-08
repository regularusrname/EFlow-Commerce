using FluentValidation;

namespace Orders.API.Features.GetOrder;

public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator()
    {
        RuleFor(q => q.OrderId)
            .Must(id => Guid.TryParse(id, out var _))
            .WithMessage("OrderId has invalid GUID format");
    }

}
