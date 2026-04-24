using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Categories.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public async Task<Result<GetCategoryByIdResponse>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<GetCategoryByIdResponse>(
                Error.NotFound(nameof(Category), request.CategoryId));

        return Result.Success(category.ToGetByIdResponse());
    }
}