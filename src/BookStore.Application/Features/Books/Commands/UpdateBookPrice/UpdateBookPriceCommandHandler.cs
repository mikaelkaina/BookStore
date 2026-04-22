using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Books.Commands.UpdateBookPrice;

public sealed class UpdateBookPriceCommandHandler : IRequestHandler<UpdateBookPriceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookRepository _bookRepository;

    public UpdateBookPriceCommandHandler(IUnitOfWork unitOfWork,
        IBookRepository bookRepository)
    {
        _unitOfWork = unitOfWork;
        _bookRepository = bookRepository;
    }

    public async Task<Result> Handle(UpdateBookPriceCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
            return Result.Failure(BookErrors.NotFound(request.BookId));

        var result = book.UpdatePrice(request.NewPrice);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _bookRepository.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
