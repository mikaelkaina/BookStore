using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, Result<UpdateBookResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateBookCommandHandler(IUnitOfWork unitOfWork, 
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<UpdateBookResponse>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (book is null)
            return Result.Failure<UpdateBookResponse>(
                BookErrors.NotFound(request.BookId));

        var result = book.UpdateDetails(
            request.Title,
            request.Author,
            request.Description,
            request.CoverImageUrl,
            request.Publisher
        );

        if (result.IsFailure)
            return Result.Failure<UpdateBookResponse>(result.Error);

        var category = await _categoryRepository.GetByIdAsync(book.CategoryId, cancellationToken);
        var categoryName = category?.Name ?? string.Empty;

        await _bookRepository.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(book.ToUpdateResponse(categoryName));
    }
}
