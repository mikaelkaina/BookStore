using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(OrderErrors.NotFound(request.OrderId));

        var result = order.Cancel(request.Reason);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
