using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Carts.Commands.RemoveItemFromCart;

public sealed record RemoveItemFromCartCommand(
    Guid CartId,
    Guid BookId) : ICommand<Result>;