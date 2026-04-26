using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId)
    : IQuery<Result<GetOrderByIdResponse>>;