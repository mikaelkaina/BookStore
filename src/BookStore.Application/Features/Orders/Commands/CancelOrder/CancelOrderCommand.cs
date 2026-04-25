using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(
    Guid OrderId,
    string Reason) : ICommand<Result>;
