using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode,
    string? Notes,
    IEnumerable<OrderItemRequest> Items) : ICommand<Result<CreateOrderResponse>>;

public sealed record OrderItemRequest(
    Guid BookId,
    int Quantity
);

