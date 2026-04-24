using BookStore.Domain.Common;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Categories.Queries.GetAllCategories;

public sealed class GetAllCategoriesQueryHandler
    : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<GetAllCategoriesResponse>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<Result<IEnumerable<GetAllCategoriesResponse>>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetActiveAsync(cancellationToken);
        return Result.Success(categories.Select(c => c.ToGetAllResponse()));
    }
}
