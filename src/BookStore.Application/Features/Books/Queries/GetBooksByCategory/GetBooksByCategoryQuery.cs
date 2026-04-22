using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Queries.GetBooksByCategory;

public sealed record GetBooksByCategoryQuery(Guid CategoryId)
    : IQuery<Result<IEnumerable<GetBooksByCategoryResponse>>>;
