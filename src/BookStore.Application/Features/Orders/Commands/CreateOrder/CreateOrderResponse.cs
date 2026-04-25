namespace BookStore.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string Status,
    decimal SubTotal,
    decimal ShippingCost,
    decimal Discount,
    decimal Total,
    string Currency,
    IEnumerable<OrderItemResponse> Items,
    DateTime CreatedAt
    );

public sealed record OrderItemResponse(
    Guid BookId,
    string BookTitle,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice,
    string Currency
);