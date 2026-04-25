using FluentValidation;

namespace BookStore.Application.Features.Orders.Commands.StartOrderProcessing;

public sealed class StartOrderProcessingCommandValidator
    : AbstractValidator<StartOrderProcessingCommand>
{
    public StartOrderProcessingCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
    }
}

