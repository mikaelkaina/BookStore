using FluentValidation;

namespace BookStore.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.");

        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Number is required.");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("Neighborhood is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .Length(2).WithMessage("State must be a 2-letter code.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must have at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("BookId is required.");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}
