using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Books.Commands.ManageStock;

public sealed class AddStockCommandHandler : IRequestHandler<AddStockCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookRepository _bookRepository;

    public AddStockCommandHandler(IUnitOfWork unitOfWork,
        IBookRepository bookRepository)
    {
        _unitOfWork = unitOfWork;
        _bookRepository = bookRepository;
    }

    public async Task<Result> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null) 
            return Result.Failure(BookErrors.NotFound(request.BookId));

        var result = book.AddStock(request.Quantity);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _bookRepository.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();    
    }
}

public sealed class DecrementStockCommandHandler : IRequestHandler<DecrementStockCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookRepository _bookRepository;
    public DecrementStockCommandHandler(IUnitOfWork unitOfWork,
        IBookRepository bookRepository)
    {
        _unitOfWork = unitOfWork;
        _bookRepository = bookRepository;
    }
    public async Task<Result> Handle(DecrementStockCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null) 
            return Result.Failure(BookErrors.NotFound(request.BookId));

        var result = book.DecrementStock(request.Quantity);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _bookRepository.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();    
    }
}
