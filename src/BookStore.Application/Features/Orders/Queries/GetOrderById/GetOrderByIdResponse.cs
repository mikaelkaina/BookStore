using BookStore.Application.Features.Orders.Shared;

namespace BookStore.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string Status,
    decimal SubTotal,
    decimal ShippingCost,
    decimal Discount,
    decimal Total,
    string Currency,
    ShippingAddressResponse ShippingAddress,
    IEnumerable<OrderItemResponse> Items,
    string? Notes,
    DateTime? ShippedAt,
    DateTime? DeliveredAt,
    DateTime? CancelledAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public sealed record ShippingAddressResponse(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode
);
