using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Books.Queries.GetBookById;

public sealed class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Result<GetBookByIdResponse>>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
        => _bookRepository = bookRepository;

    public async Task<Result<GetBookByIdResponse>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
            return Result.Failure<GetBookByIdResponse>(
                BookErrors.NotFound(request.BookId));

        return Result.Success(book.ToGetByIdResponse());
    }
}
