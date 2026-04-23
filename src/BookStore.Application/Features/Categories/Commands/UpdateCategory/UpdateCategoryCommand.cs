using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Description) : ICommand<Result<UpdateCategoryResponse>>;