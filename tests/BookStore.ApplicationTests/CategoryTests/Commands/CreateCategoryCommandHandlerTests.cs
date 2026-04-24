using BookStore.Application.Features.Categories.Commands.CreateCategory;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CategoryTests.Commands;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new CreateCategoryCommandHandler(
            _categoryRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = new CreateCategoryCommand("Tech", "Tecnologia");

        _categoryRepository
            .Setup(x => x.SlugExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _categoryRepository.Verify(
            x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidName_ShouldFail()
    {
        var command = new CreateCategoryCommand("", "Desc");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithExistingSlug_ShouldFail()
    {
        var command = new CreateCategoryCommand("Tech", "Tecnologia");

        _categoryRepository
            .Setup(x => x.SlugExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("SlugExists");
    }

    [Fact]
    public async Task Handle_WhenSlugExists_ShouldNotPersist()
    {
        var command = new CreateCategoryCommand("Tech", "Tecnologia");

        _categoryRepository
            .Setup(x => x.SlugExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _categoryRepository.Verify(
            x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}