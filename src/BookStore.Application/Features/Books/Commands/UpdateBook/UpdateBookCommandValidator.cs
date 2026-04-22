using FluentValidation;

namespace BookStore.Application.Features.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required.");

        RuleFor(x => x.Publisher)
            .NotEmpty().WithMessage("Publisher is required.");
    }
}