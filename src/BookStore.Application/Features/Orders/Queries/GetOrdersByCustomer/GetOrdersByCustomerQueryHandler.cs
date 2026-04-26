using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Orders.Queries.GetOrdersByCustomer;

public sealed class GetOrdersByCustomerQueryHandler
    : IRequestHandler<GetOrdersByCustomerQuery, Result<IEnumerable<GetOrdersByCustomerResponse>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetOrdersByCustomerQueryHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<IEnumerable<GetOrdersByCustomerResponse>>> Handle(
        GetOrdersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<IEnumerable<GetOrdersByCustomerResponse>>(
                CustomerErrors.NotFound(request.CustomerId));

        var orders = await _orderRepository.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        return Result.Success(orders.Select(o => o.ToGetByCustomerResponse()));
    }
}