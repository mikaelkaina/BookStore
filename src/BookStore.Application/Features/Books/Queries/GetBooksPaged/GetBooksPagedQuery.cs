using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Books.Queries.GetBooksPaged;

public sealed record GetBooksPagedQuery(
    string? SearchTerm,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? SortByPrice,
    bool Ascending = true,
    int Page = 1,
    int PageSize = 20) : IQuery<Result<PagedResponse<GetBooksPagedResponse>>>;
