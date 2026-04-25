using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Commands.ApplyDiscount;

public sealed record ApplyDiscountCommand(
    Guid OrderId,
    decimal DiscountAmount) : ICommand<Result>;
