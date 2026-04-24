using BookStore.Application.Features.Categories.Queries.GetAllCategories;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CategoryTests.Queries;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly GetAllCategoriesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();

        _handler = new GetAllCategoriesQueryHandler(
            _categoryRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithCategories_ShouldReturnList()
    {
        var categories = new List<Category>
        {
            Category.Create("Tech", "desc").Value,
            Category.Create("Books", "desc").Value
        };

        _categoryRepository
            .Setup(x => x.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetAllCategoriesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNoCategories_ShouldReturnEmptyList()
    {
        _categoryRepository
            .Setup(x => x.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetAllCategoriesQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        _categoryRepository
            .Setup(x => x.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetAllCategoriesQuery();

        await _handler.Handle(query, CancellationToken.None);

        _categoryRepository.Verify(
            x => x.GetActiveAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}