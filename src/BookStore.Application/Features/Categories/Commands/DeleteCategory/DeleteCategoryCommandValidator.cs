using FluentValidation;

namespace BookStore.Application.Features.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");
    }
}
