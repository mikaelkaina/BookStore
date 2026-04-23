using BookStore.Application.Features.Categories.Commands.CreateCategory;
using BookStore.Application.Features.Categories.Commands.UpdateCategory;
using BookStore.Domain.Entities;

namespace BookStore.Application.Features.Categories;

public static class CategoryMappingExtensions
{
    public static CreateCategoryResponse ToCreateResponse(this Category category) =>
        new(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IsActive,
            category.CreatedAt
        );

    public static UpdateCategoryResponse ToUpdateResponse(this Category category) =>
       new(
           category.Id,
           category.Name,
           category.Slug,
           category.Description,
           category.IsActive,
           category.UpdatedAt
       );

}
