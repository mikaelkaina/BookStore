using FluentValidation;

namespace BookStore.Application.Features.Carts.Commands.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandValidator
    : AbstractValidator<UpdateItemQuantityCommand>
{
    public UpdateItemQuantityCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("CartId is required.");

        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
    }
}