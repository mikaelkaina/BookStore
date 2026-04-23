namespace BookStore.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    DateTime CreatedAt
);