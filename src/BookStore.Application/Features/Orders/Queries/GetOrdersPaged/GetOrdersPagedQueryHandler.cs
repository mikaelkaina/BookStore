using BookStore.Application.Common;
using BookStore.Domain.Common;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Orders.Queries.GetOrdersPaged;

public sealed class GetOrdersPagedQueryHandler
    : IRequestHandler<GetOrdersPagedQuery, Result<PagedResponse<GetOrdersPagedResponse>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersPagedQueryHandler(IOrderRepository orderRepository)
        => _orderRepository = orderRepository;

    public async Task<Result<PagedResponse<GetOrdersPagedResponse>>> Handle(
        GetOrdersPagedQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page <= 0)
            return Result.Failure<PagedResponse<GetOrdersPagedResponse>>(
                Error.Validation(nameof(request.Page), "Page must be greater than zero."));

        if (request.PageSize <= 0 || request.PageSize > 100)
            return Result.Failure<PagedResponse<GetOrdersPagedResponse>>(
                Error.Validation(nameof(request.PageSize), "PageSize must be between 1 and 100."));

        var (orders, totalCount) = await _orderRepository.GetPagedAsync(
            request.CustomerId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var response = new PagedResponse<GetOrdersPagedResponse>(
            orders.Select(o => o.ToGetPagedResponse()),
            totalCount,
            request.Page,
            request.PageSize,
            totalPages,
            HasNextPage: request.Page < totalPages,
            HasPreviousPage: request.Page > 1);

        return Result.Success(response);
    }
}