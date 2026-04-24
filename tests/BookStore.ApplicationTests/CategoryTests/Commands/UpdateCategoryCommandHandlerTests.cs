using BookStore.Application.Features.Categories.Commands.UpdateCategory;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CategoryTests.Commands;

public class UpdateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _categoryRepository = new Mock<ICategoryRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new UpdateCategoryCommandHandler(
            _categoryRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var category = Category.Create("Old", "desc").Value;

        var command = new UpdateCategoryCommand(
            category.Id,
            "Tech",
            "Nova descrição"
        );

        _categoryRepository
            .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepository
            .Setup(x => x.SlugExistsAsync(It.IsAny<string>(), category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _categoryRepository.Verify(
            x => x.UpdateAsync(category, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldFail()
    {
        var command = new UpdateCategoryCommand(
            Guid.NewGuid(),
            "Tech",
            "Desc"
        );

        _categoryRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenSlugExists_ShouldFail()
    {
        var category = Category.Create("Old", "desc").Value;

        var command = new UpdateCategoryCommand(
            category.Id,
            "Tech",
            "Desc"
        );

        _categoryRepository
            .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepository
            .Setup(x => x.SlugExistsAsync(It.IsAny<string>(), category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _categoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldFail()
    {
        var category = Category.Create("Old", "desc").Value;

        var command = new UpdateCategoryCommand(
            category.Id,
            "",
            "Desc"
        );

        _categoryRepository
            .Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _categoryRepository
            .Setup(x => x.SlugExistsAsync(It.IsAny<string>(), category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _categoryRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
