using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.DeliverOrder;

public sealed record DeliverOrderCommand(Guid OrderId) : ICommand<Result>;