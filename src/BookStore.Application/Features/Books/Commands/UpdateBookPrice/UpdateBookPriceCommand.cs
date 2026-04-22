using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Commands.UpdateBookPrice;

public sealed record UpdateBookPriceCommand(
    Guid BookId,
    decimal NewPrice
) : ICommand<Result>;