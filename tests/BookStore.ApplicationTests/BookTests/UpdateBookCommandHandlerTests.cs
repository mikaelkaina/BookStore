using BookStore.Application.Features.Books.Commands.UpdateBook;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests;

public class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly UpdateBookCommandHandler _handler;

    public UpdateBookCommandHandlerTests()
    {
        _handler = new UpdateBookCommandHandler(
            _unitOfWork.Object,
            _bookRepository.Object,
            _categoryRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = new UpdateBookCommand(
            Guid.NewGuid(),
            "Clean Code",
            "Robert C. Martin",
            "Desc",
            null,
            "Prentice Hall"
        );

        var bookResult = Book.Create(
            "Old",
            "Old",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Old",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        var book = bookResult.Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var categoryResult = Category.Create("Tech", "tech");
        var category = categoryResult.Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(book.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _bookRepository.Verify(x =>
            x.UpdateAsync(book, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookNotFound_ShouldFail()
    {
        var command = new UpdateBookCommand(
            Guid.NewGuid(),
            "Title",
            "Author",
            null,
            null,
            "Publisher"
        );

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _bookRepository.Verify(x => x.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenDomainFails_ShouldFail()
    {
        var command = new UpdateBookCommand(
            Guid.NewGuid(),
            "",
            "Author",
            null,
            null,
            "Publisher"
        );

        var bookResult = Book.Create(
            "Old",
            "Old",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Old",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        var book = bookResult.Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}