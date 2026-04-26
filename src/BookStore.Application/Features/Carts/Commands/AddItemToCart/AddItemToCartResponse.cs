using BookStore.Application.Features.Carts.Shared;

namespace BookStore.Application.Features.Carts.Commands.AddItemToCart;

public sealed record AddItemToCartResponse(
    Guid Id,
    Guid? CustomerId,
    string? SessionId,
    IEnumerable<CartItemResponse> Items,
    decimal Total,
    string Currency,
    int TotalItems,
    DateTime? ExpiresAt
);