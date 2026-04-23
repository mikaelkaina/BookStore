using BookStore.Application.Features.Books.Queries.GetBooksByCategory;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests.Queries;

public class GetBooksByCategoryQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly GetBooksByCategoryQueryHandler _handler;

    public GetBooksByCategoryQueryHandlerTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _categoryRepository = new Mock<ICategoryRepository>();

        _handler = new GetBooksByCategoryQueryHandler(
            _bookRepository.Object,
            _categoryRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidCategory_ShouldReturnBooks()
    {
        var category = Category.Create("Tech", "tech").Value;

        var books = new List<Book>
    {
        Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000,1,1),
            BookFormat.Paperback,
            "EN",
            category.Id
        ).Value
    };

        _categoryRepository
            .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _bookRepository
            .Setup(x => x.GetByCategoryAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(books);

        var query = new GetBooksByCategoryQuery(category.Id);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldFail()
    {
        var categoryId = Guid.NewGuid();

        _categoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var query = new GetBooksByCategoryQuery(categoryId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _bookRepository.Verify(
            x => x.GetByCategoryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoBooks_ShouldReturnEmptyList()
    {
        var category = Category.Create("Tech", "tech").Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _bookRepository
            .Setup(x => x.GetByCategoryAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());

        var query = new GetBooksByCategoryQuery(category.Id);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}