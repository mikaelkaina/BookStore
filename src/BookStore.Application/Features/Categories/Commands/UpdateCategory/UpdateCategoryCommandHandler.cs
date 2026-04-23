using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateCategoryResponse>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<UpdateCategoryResponse>(
                Error.NotFound(nameof(Category), request.CategoryId));

        var newSlug = request.Name.Trim().ToLowerInvariant().Replace(" ", "-");
        var slugExists = await _categoryRepository.SlugExistsAsync(
            newSlug, excludeId: request.CategoryId, cancellationToken: cancellationToken);

        if (slugExists)
            return Result.Failure<UpdateCategoryResponse>(
                Error.Conflict("Category.SlugExists",
                    $"A category with name '{request.Name}' already exists."));

        var result = category.Update(request.Name, request.Description);
        if (result.IsFailure)
            return Result.Failure<UpdateCategoryResponse>(result.Error);

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(category.ToUpdateResponse());
    }
}