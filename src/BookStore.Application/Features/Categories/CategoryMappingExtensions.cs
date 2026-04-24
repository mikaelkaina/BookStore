using BookStore.Application.Features.Categories.Commands.CreateCategory;
using BookStore.Application.Features.Categories.Commands.UpdateCategory;
using BookStore.Application.Features.Categories.Queries.GetAllCategories;
using BookStore.Application.Features.Categories.Queries.GetCategoryById;
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

    public static GetCategoryByIdResponse ToGetByIdResponse(this Category category) =>
        new(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IsActive,
            category.CreatedAt,
            category.UpdatedAt
        );

    public static GetAllCategoriesResponse ToGetAllResponse(this Category category) =>
       new(
           category.Id,
           category.Name,
           category.Slug,
           category.Description
       );

}
