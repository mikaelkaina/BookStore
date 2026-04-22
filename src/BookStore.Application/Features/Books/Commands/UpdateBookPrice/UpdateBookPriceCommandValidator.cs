using FluentValidation;

namespace BookStore.Application.Features.Books.Commands.UpdateBookPrice;

public sealed class UpdateBookPriceCommandValidator : AbstractValidator<UpdateBookPriceCommand>
{
    public UpdateBookPriceCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required.");

        RuleFor(x => x.NewPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
    }
}
