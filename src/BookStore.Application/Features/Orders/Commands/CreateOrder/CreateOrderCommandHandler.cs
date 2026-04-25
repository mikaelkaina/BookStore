using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using MediatR;

namespace BookStore.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateOrderCommandHandler(IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IBookRepository bookRepository,
        ICustomerRepository customerRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _bookRepository = bookRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<CreateOrderResponse>(
                CustomerErrors.NotFound(request.CustomerId));

        var addressResult = Address.Create(
            request.Street,
            request.Number,
            request.Complement,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode);

        if (addressResult.IsFailure)
            return Result.Failure<CreateOrderResponse>(addressResult.Error);

        var orderResult = Order.Create(
            request.CustomerId,
            addressResult.Value,
            request.Notes);

        if (orderResult.IsFailure)
            return Result.Failure<CreateOrderResponse>(orderResult.Error);

        var order = orderResult.Value;

        foreach(var item in request.Items)
        {
            var book = await _bookRepository.GetByIdAsync(item.BookId, cancellationToken);
            if (book is null)
                return Result.Failure<CreateOrderResponse>(
                    BookErrors.NotFound(item.BookId));

            var addResult = order.AddItem(book, item.Quantity);
            if (addResult.IsFailure)
                return Result.Failure<CreateOrderResponse>(addResult.Error);

            var decrementResult = book.DecrementStock(item.Quantity);
            if (decrementResult.IsFailure)
                return Result.Failure<CreateOrderResponse>(decrementResult.Error);

            await _bookRepository.UpdateAsync(book, cancellationToken);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(order.ToCreateOrderResponse());
    }
}
