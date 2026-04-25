using FluentValidation;

namespace BookStore.Application.Features.Orders.Commands.ApplyDiscount;

public sealed class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    public ApplyDiscountCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");

        RuleFor(x => x.DiscountAmount)
            .GreaterThan(0).WithMessage("Discount amount must be greater than zero.");
    }
}
