using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Orders.Commands.ConfirmOrderPayment;

public sealed record ConfirmOrderPaymentCommandHandler : IRequestHandler<ConfirmOrderPaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public ConfirmOrderPaymentCommandHandler(IUnitOfWork unitOfWork,
        IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(ConfirmOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(OrderErrors.NotFound(request.OrderId));

        var result = order.ConfirmPayment();
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
