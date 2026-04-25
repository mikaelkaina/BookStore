using FluentValidation;

namespace BookStore.Application.Features.Orders.Commands.ConfirmOrderPayment;

public sealed class ConfirmOrderPaymentCommandValidator : AbstractValidator<ConfirmOrderPaymentCommand>
{
    public ConfirmOrderPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");
    }
}
