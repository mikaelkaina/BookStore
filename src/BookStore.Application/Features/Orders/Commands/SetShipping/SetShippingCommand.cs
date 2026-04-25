using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;
namespace BookStore.Application.Features.Orders.Commands.SetShipping;

public sealed record SetShippingCommand(
    Guid OrderId,
    decimal ShippingCost) : ICommand<Result>;
