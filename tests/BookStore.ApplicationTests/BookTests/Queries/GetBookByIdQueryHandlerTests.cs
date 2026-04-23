using BookStore.Application.Features.Books.Queries.GetBookById;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests.Queries;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _handler = new GetBookByIdQueryHandler(_bookRepository.Object);
    }

    [Fact]
    public async Task Handle_WithExistingBook_ShouldReturnSuccess()
    {
        var bookId = Guid.NewGuid();

        var bookResult = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2008, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        var book = bookResult.Value;

        var categoryResult = Category.Create("Tech", "tech");
        var category = categoryResult.Value;
        book.SetCategory(category);

        _bookRepository
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var query = new GetBookByIdQuery(bookId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(book.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistingBook_ShouldFail()
    {
        var bookId = Guid.NewGuid();

        _bookRepository
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var query = new GetBookByIdQuery(bookId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");
    }
}
