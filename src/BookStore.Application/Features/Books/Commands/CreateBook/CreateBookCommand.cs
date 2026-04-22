using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;
using BookStore.Domain.Enums;

namespace BookStore.Application.Features.Books.Commands.CreateBook;

public sealed record CreateBookCommand(
    string Title,
    string Author,
    string? Description,
    string Isbn,
    decimal Price,
    int StockQuantity,
    int PageCount,
    string? CoverImageUrl,
    string Publisher,
    DateOnly PublishedDate,
    BookFormat Format,
    string Language,
    Guid CategoryId) : ICommand<Result<CreateBookResponse>>;
