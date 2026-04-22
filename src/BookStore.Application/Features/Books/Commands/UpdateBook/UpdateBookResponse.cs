namespace BookStore.Application.Features.Books.Commands.UpdateBook;

public sealed record UpdateBookResponse(
    Guid Id,
    string Title,
    string Author,
    string? Description,
    string? CoverImageUrl,
    string Publisher,
    string CategoryName,
    DateTime? UpdatedAt
);