using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Commands.ManageStock;

public sealed record AddStockCommand(
    Guid BookId,
    int Quantity
) : ICommand<Result>;

public sealed record DecrementStockCommand(
    Guid BookId,
    int Quantity
) : ICommand<Result>;
