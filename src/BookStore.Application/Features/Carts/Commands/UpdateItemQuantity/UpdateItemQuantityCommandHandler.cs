using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Carts.Commands.UpdateItemQuantity;

public sealed class UpdateItemQuantityCommandHandler
    : IRequestHandler<UpdateItemQuantityCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItemQuantityCommandHandler(
        ICartRepository cartRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
            return Result.Failure(CartErrors.NotFound(request.CartId));

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
            return Result.Failure(BookErrors.NotFound(request.BookId));

        var result = cart.UpdateItemQuantity(request.BookId, request.Quantity, book.StockQuantity);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _cartRepository.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
