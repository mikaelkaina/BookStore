using BookStore.Application.Common;
using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;
using BookStore.Domain.Enums;

namespace BookStore.Application.Features.Orders.Queries.GetOrdersPaged;

public sealed record GetOrdersPagedQuery(
    Guid? CustomerId,
    OrderStatus? Status,
    int Page = 1,
    int PageSize = 20) : IQuery<Result<PagedResponse<GetOrdersPagedResponse>>>;