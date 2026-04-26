using BookStore.Application.Common;
using BookStore.Domain.Common;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Books.Queries.GetBooksPaged;

public sealed class GetBooksPagedQueryHandler
    : IRequestHandler<GetBooksPagedQuery, Result<PagedResponse<GetBooksPagedResponse>>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksPagedQueryHandler(IBookRepository bookRepository)
        => _bookRepository = bookRepository;

    public async Task<Result<PagedResponse<GetBooksPagedResponse>>> Handle(
        GetBooksPagedQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page <= 0)
            return Result.Failure<PagedResponse<GetBooksPagedResponse>>(
                Error.Validation(nameof(request.Page), "Page must be greater than zero."));

        if (request.PageSize <= 0 || request.PageSize > 100)
            return Result.Failure<PagedResponse<GetBooksPagedResponse>>(
                Error.Validation(nameof(request.PageSize), "PageSize must be between 1 and 100."));

        var (books, totalCount) = await _bookRepository.GetPagedAsync(
            request.SearchTerm,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.SortByPrice,
            request.Ascending,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var response = new PagedResponse<GetBooksPagedResponse>(
            books.Select(b => b.ToGetPagedResponse()),
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            HasNextPage: request.Page < totalPages,
            HasPreviousPage: request.Page > 1);

        return Result.Success(response);
    }
}