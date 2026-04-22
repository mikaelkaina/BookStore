using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using MediatR;

namespace BookStore.Application.Features.Books.Commands.CreateBook;

public sealed class CreateBookCommandHandler
    : IRequestHandler<CreateBookCommand, Result<CreateBookResponse>>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookCommandHandler(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateBookResponse>> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<CreateBookResponse>(
                Error.NotFound(nameof(Category), request.CategoryId));

        var isbnResult = Isbn.Create(request.Isbn);
        if (isbnResult.IsFailure)
            return Result.Failure<CreateBookResponse>(isbnResult.Error);

        var isbnExists = await _bookRepository.IsbnExistsAsync(
            isbnResult.Value, cancellationToken: cancellationToken);

        if (isbnExists)
            return Result.Failure<CreateBookResponse>(
                BookErrors.IsbnAlreadyExists(request.Isbn));

        var bookResult = Book.Create(
            request.Title,
            request.Author,
            request.Description,
            request.Isbn,
            request.Price,
            request.StockQuantity,
            request.PageCount,
            request.CoverImageUrl,
            request.Publisher,
            request.PublishedDate,
            request.Format,
            request.Language,
            request.CategoryId);

        if (bookResult.IsFailure)
            return Result.Failure<CreateBookResponse>(bookResult.Error);

        await _bookRepository.AddAsync(bookResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(bookResult.Value.ToCreateResponse(category.Name));
    }
}