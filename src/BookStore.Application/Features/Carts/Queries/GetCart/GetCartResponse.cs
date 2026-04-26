using BookStore.Application.Features.Carts.Shared;

namespace BookStore.Application.Features.Carts.Queries.GetCart;

public sealed record GetCartResponse(
    Guid Id,
    Guid? CustomerId,
    string? SessionId,
    IEnumerable<CartItemResponse> Items,
    decimal Total,
    string Currency,
    int TotalItems,
    bool IsCheckedOut,
    DateTime? ExpiresAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
