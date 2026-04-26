using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Carts.Commands.AddItemToCart;

public sealed record AddItemToCartCommand(
    Guid? CustomerId,
    string? SessionId,
    Guid BookId,
    int Quantity) : ICommand<Result<AddItemToCartResponse>>;
