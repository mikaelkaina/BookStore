using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Carts.Commands.CheckoutCart;

public sealed record CheckoutCartCommand(Guid CartId) : ICommand<Result>;