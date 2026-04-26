using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Carts.Commands.RemoveItemFromCart;

public sealed class RemoveItemFromCartCommandHandler
    : IRequestHandler<RemoveItemFromCartCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItemFromCartCommandHandler(
        ICartRepository cartRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveItemFromCartCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
            return Result.Failure(CartErrors.NotFound(request.CartId));

        var result = cart.RemoveItem(request.BookId);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
