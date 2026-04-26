namespace BookStore.Application.Features.Books.Queries.GetBooksPaged;

public sealed record GetBooksPagedResponse(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    decimal Price,
    string Currency,
    int StockQuantity,
    string? CoverImageUrl,
    string Format,
    string CategoryName
);
