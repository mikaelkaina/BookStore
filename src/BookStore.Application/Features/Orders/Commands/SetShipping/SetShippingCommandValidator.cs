using FluentValidation;

namespace BookStore.Application.Features.Orders.Commands.SetShipping;

public sealed class SetShippingCommandValidator : AbstractValidator<SetShippingCommand>
{
    public SetShippingCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("Shipping cost cannot be negative.");
    }
}