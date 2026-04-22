using FluentValidation;

namespace BookStore.Application.Features.Books.Commands.CreateBook;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.");

        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be greater than zero.");

        RuleFor(x => x.Publisher)
            .NotEmpty().WithMessage("Publisher is required.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}
