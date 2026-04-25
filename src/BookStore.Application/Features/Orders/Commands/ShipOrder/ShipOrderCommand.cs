using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.ShipOrder;

public sealed record ShipOrderCommand(Guid OrderId) : ICommand<Result>;
