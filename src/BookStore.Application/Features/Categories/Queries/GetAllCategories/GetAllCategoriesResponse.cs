namespace BookStore.Application.Features.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesResponse(
    Guid Id,
    string Name,
    string Slug,
    string? Description
);

