namespace BookStore.Application.Features.Orders.Queries.GetOrdersPaged;

public sealed record GetOrdersPagedResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string Status,
    decimal Total,
    string Currency,
    int ItemCount,
    DateTime CreatedAt);