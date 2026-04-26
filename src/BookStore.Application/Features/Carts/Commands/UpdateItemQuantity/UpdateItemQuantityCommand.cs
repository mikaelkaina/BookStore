using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Carts.Commands.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(
    Guid CartId,
    Guid BookId,
    int Quantity) : ICommand<Result>;
