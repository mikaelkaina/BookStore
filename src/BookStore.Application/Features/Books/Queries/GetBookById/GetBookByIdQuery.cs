using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery(Guid BookId) : IQuery<Result<GetBookByIdResponse>>;
