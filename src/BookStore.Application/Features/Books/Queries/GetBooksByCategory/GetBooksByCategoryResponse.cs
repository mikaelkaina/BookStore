namespace BookStore.Application.Features.Books.Queries.GetBooksByCategory;

public sealed record GetBooksByCategoryResponse(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    decimal Price,
    string Currency,
    int StockQuantity,
    string? CoverImageUrl,
    string Format
);
