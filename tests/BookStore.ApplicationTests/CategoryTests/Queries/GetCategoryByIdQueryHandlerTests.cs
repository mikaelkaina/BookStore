using BookStore.Application.Features.Categories.Queries.GetCategoryById;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CategoryTests.Queries;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();

        _handler = new GetCategoryByIdQueryHandler(
            _categoryRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingCategory_ShouldReturnSuccess()
    {
        var categoryId = Guid.NewGuid();

        var categoryResult = Category.Create("Tech", "desc");
        categoryResult.IsSuccess.Should().BeTrue();

        var category = categoryResult.Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var query = new GetCategoryByIdQuery(categoryId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(category.Id);
        result.Value.Name.Should().Be("Tech");
    }

    [Fact]
    public async Task Handle_WithNonExistingCategory_ShouldReturnFailure()
    {
        var categoryId = Guid.NewGuid();

        _categoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var query = new GetCategoryByIdQuery(categoryId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCallRepository()
    {
        var categoryId = Guid.NewGuid();

        _categoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var query = new GetCategoryByIdQuery(categoryId);

        await _handler.Handle(query, CancellationToken.None);

        _categoryRepository.Verify(
            x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}