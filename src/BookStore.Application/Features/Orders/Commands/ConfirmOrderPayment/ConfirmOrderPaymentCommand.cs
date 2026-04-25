using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.ConfirmOrderPayment;

public sealed record ConfirmOrderPaymentCommand(Guid OrderId) : ICommand<Result>;
