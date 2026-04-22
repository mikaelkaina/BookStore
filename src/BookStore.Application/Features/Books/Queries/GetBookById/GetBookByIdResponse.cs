namespace BookStore.Application.Features.Books.Queries.GetBookById;

public sealed record GetBookByIdResponse(
    Guid Id,
    string Title,
    string Author,
    string? Description,
    string Isbn,
    decimal Price,
    string Currency,
    int StockQuantity,
    int PageCount,
    string? CoverImageUrl,
    string Publisher,
    DateOnly PublishedDate,
    string Format,
    string Language,
    bool IsActive,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
