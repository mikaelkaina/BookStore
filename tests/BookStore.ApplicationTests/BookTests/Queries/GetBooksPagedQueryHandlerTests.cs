using BookStore.Application.Features.Books.Queries.GetBooksPaged;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests.Queries;

public class GetBooksPagedQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly GetBooksPagedQueryHandler _handler;

    public GetBooksPagedQueryHandlerTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _handler = new GetBooksPagedQueryHandler(_bookRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPagedResult()
    {
        var query = new GetBooksPagedQuery(
            SearchTerm: null,
            CategoryId: null,
            MinPrice: null,
            MaxPrice: null,
            SortByPrice: null,
            Ascending: true,
            Page: 1,
            PageSize: 2
        );

        var category = Category.Create("Tech", "tech").Value;

        var books = new List<Book>
    {
        Book.Create(
            "Book 1", "Author", null, "9780132350884",
            50, 10, 200, null, "Pub",
            new DateOnly(2000,1,1), BookFormat.Paperback, "EN",
            category.Id
        ).Value,

        Book.Create(
            "Book 2", "Author", null, "9780306406157",
            60, 5, 150, null, "Pub",
            new DateOnly(2001,1,1), BookFormat.Ebook, "EN",
            category.Id
        ).Value
    };

        // ⚠️ setar Category (mesmo problema do outro teste)
        foreach (var b in books)
        {
            typeof(Book).GetProperty("Category")!
                .SetValue(b, category);
        }

        _bookRepository
            .Setup(x => x.GetPagedAsync(
                query.SearchTerm,
                query.CategoryId,
                query.MinPrice,
                query.MaxPrice,
                query.SortByPrice,
                query.Ascending,
                query.Page,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((books, 2));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var response = result.Value;

        response.Items.Should().HaveCount(2);
        response.TotalCount.Should().Be(2);
        response.Page.Should().Be(1);
        response.TotalPages.Should().Be(1);
        response.HasNextPage.Should().BeFalse();
        response.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithInvalidPage_ShouldFail()
    {
        var query = new GetBooksPagedQuery(
            null, null, null, null, null, true, 0, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidPageSize_ShouldFail()
    {
        var query = new GetBooksPagedQuery(
            null, null, null, null, null, true, 1, 0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoResults_ShouldReturnEmptyList()
    {
        var query = new GetBooksPagedQuery(
            null, null, null, null, null, true, 1, 10);

        _bookRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Book>(), 0));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}
