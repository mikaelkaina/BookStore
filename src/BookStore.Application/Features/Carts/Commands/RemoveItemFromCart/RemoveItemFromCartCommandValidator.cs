using FluentValidation;

namespace BookStore.Application.Features.Carts.Commands.RemoveItemFromCart;

public sealed class RemoveItemFromCartCommandValidator
    : AbstractValidator<RemoveItemFromCartCommand>
{
    public RemoveItemFromCartCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty().WithMessage("CartId is required.");

        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required.");
    }
}
