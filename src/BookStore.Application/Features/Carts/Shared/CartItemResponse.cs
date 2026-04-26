namespace BookStore.Application.Features.Carts.Shared;

public sealed record CartItemResponse(
    Guid BookId,
    string BookTitle,
    string? BookCoverUrl,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    string Currency
);