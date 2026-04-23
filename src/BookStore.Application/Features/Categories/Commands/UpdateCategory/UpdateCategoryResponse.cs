namespace BookStore.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    DateTime? UpdatedAt);
