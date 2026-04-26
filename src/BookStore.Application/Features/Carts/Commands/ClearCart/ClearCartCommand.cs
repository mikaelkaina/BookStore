using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Carts.Commands.ClearCart;

public sealed record ClearCartCommand(Guid CartId) : ICommand<Result>;
