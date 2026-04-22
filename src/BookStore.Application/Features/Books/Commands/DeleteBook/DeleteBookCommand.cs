using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(Guid BookId) : ICommand<Result>;