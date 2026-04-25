using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using MediatR;

namespace BookStore.Application.Features.Orders.Commands.SetShipping;

public sealed class SetShippingCommandHandler
    : IRequestHandler<SetShippingCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetShippingCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SetShippingCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(OrderErrors.NotFound(request.OrderId));

        var shippingResult = Money.Create(request.ShippingCost);
        if (shippingResult.IsFailure)
            return Result.Failure(shippingResult.Error);

        var result = order.SetShipping(shippingResult.Value);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}