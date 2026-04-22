using FluentValidation;

namespace BookStore.Application.Features.Books.Commands.DeleteBook;

public sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty().WithMessage("BookId is required.");
    }
}
