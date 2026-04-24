using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesQuery()
    : IQuery<Result<IEnumerable<GetAllCategoriesResponse>>>;