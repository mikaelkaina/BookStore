using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Commands.UpdateBook;

public sealed record UpdateBookCommand(
    Guid BookId,
    string Title,
    string Author,
    string? Description,
    string? CoverImageUrl,
    string Publisher) : ICommand<Result<UpdateBookResponse>>;