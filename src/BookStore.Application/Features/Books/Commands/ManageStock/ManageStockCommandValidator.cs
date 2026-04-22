using FluentValidation;

namespace BookStore.Application.Features.Books.Commands.ManageStock;

public sealed class AddStockCommandValidator : AbstractValidator<AddStockCommand>
{
    public AddStockCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

public sealed class DecrementStockCommandValidator : AbstractValidator<DecrementStockCommand>
{
    public DecrementStockCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("Book ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
