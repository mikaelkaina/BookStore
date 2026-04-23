using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCategoryResponse>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var categoryResult = Category.Create(request.Name, request.Description);
        if (categoryResult.IsFailure)
            return Result.Failure<CreateCategoryResponse>(categoryResult.Error);

        var slugExists = await _categoryRepository.SlugExistsAsync(
            categoryResult.Value.Slug, cancellationToken: cancellationToken);

        if (slugExists)
            return Result.Failure<CreateCategoryResponse>(
                Error.Conflict("Category.SlugExists",
                    $"A category with name '{request.Name}' already exists."));

        await _categoryRepository.AddAsync(categoryResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(categoryResult.Value.ToCreateResponse());
    }
}