using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Carts.Commands.AddItemToCart;

public sealed record AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, Result<AddItemToCartResponse>>
{
    private readonly ICartRepository _cartRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddItemToCartCommandHandler(ICartRepository cartRepository,
        IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AddItemToCartResponse>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
            return Result.Failure<AddItemToCartResponse>(
                BookErrors.NotFound(request.BookId));

        Cart cart;
        bool isNew = false;
        
        if (request.CustomerId.HasValue)
        {
            var existing = await _cartRepository.GetByCustomerIdAsync(
                request.CustomerId.Value, cancellationToken);

            if (existing is not null)
            {
                cart = existing;

            }
            else
            {
                cart = Cart.CreateForCustomer(request.CustomerId.Value);
                isNew = true;
            }
        }
        else
        {
            var existing = await _cartRepository.GetBySessionIdAsync(
                request.SessionId!, cancellationToken);

            if (existing is not null)
            {
                cart = existing;
            }
            else
            {
                cart = Cart.CreateForGuest(request.SessionId!);
                isNew = true;
            }
        }

        var result = cart.AddItem(book, request.Quantity);
        if (result.IsFailure)
            return Result.Failure<AddItemToCartResponse>(result.Error);

        if (isNew)
            await _cartRepository.AddAsync(cart, cancellationToken);
        else
            await _cartRepository.UpdateAsync(cart, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(cart.ToAddItemResponse());
    }
}
