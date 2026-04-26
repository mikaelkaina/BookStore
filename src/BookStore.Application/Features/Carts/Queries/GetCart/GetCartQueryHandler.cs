using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Carts.Queries.GetCart;

public sealed class GetCartQueryHandler
    : IRequestHandler<GetCartQuery, Result<GetCartResponse>>
{
    private readonly ICartRepository _cartRepository;

    public GetCartQueryHandler(ICartRepository cartRepository)
        => _cartRepository = cartRepository;

    public async Task<Result<GetCartResponse>> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        Cart? cart = null;

        if (request.CustomerId.HasValue)
            cart = await _cartRepository.GetByCustomerIdAsync(
                request.CustomerId.Value, cancellationToken);
        else if (!string.IsNullOrWhiteSpace(request.SessionId))
            cart = await _cartRepository.GetBySessionIdAsync(
                request.SessionId, cancellationToken);

        if (cart is null)
            return Result.Failure<GetCartResponse>(
                new Error("Cart.NotFound", "No active cart found."));

        return Result.Success(cart.ToGetCartResponse());
    }
}
