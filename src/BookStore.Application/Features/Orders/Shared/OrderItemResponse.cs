namespace BookStore.Application.Features.Orders.Shared;

public sealed record OrderItemResponse(
    Guid BookId,
    string BookTitle,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    string Currency);