namespace BookStore.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);