using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Books.Queries.GetBooksByCategory;

public sealed class GetBooksByCategoryQueryHandler 
    : IRequestHandler<GetBooksByCategoryQuery, Result<IEnumerable<GetBooksByCategoryResponse>>>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetBooksByCategoryQueryHandler(IBookRepository bookRepository,
        ICategoryRepository categoryRepository)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IEnumerable<GetBooksByCategoryResponse>>> Handle(
        GetBooksByCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<IEnumerable<GetBooksByCategoryResponse>>(
                Error.NotFound(nameof(Category), request.CategoryId));

        var books = await _bookRepository.GetByCategoryAsync(request.CategoryId, cancellationToken);

        return Result.Success(books.Select(b => b.ToGetByCategoryResponse()));
    }
}
