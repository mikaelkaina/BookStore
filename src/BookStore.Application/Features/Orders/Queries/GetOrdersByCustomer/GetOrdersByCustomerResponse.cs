namespace BookStore.Application.Features.Orders.Queries.GetOrdersByCustomer;

public sealed record GetOrdersByCustomerResponse(
    Guid Id,
    string OrderNumber,
    string Status,
    decimal Total,
    string Currency,
    int ItemCount,
    DateTime CreatedAt
);
