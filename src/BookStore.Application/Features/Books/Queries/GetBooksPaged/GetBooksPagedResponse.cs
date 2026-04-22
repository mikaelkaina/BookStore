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

public sealed record PagedResponse<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);
