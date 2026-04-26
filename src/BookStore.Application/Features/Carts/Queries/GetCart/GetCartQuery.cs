using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Carts.Queries.GetCart;

public sealed record GetCartQuery(
    Guid? CustomerId,
    string? SessionId) : IQuery<Result<GetCartResponse>>;
