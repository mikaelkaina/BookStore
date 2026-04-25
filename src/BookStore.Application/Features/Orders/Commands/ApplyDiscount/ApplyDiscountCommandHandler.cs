using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using MediatR;

namespace BookStore.Application.Features.Orders.Commands.ApplyDiscount;

public sealed class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public ApplyDiscountCommandHandler(IUnitOfWork unitOfWork,
        IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    public async Task<Result> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(OrderErrors.NotFound(request.OrderId));

        var discountResult = Money.Create(request.DiscountAmount);
        if (discountResult.IsFailure)
            return Result.Failure(discountResult.Error);

        var result = order.ApplyDiscount(discountResult.Value);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
