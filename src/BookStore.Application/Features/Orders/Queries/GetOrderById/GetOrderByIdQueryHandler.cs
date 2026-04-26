using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, Result<GetOrderByIdResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        => _orderRepository = orderRepository;

    public async Task<Result<GetOrderByIdResponse>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure<GetOrderByIdResponse>(
                OrderErrors.NotFound(request.OrderId));

        return Result.Success(order.ToGetByIdResponse());
    }
}
