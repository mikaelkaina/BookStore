using Bogus;
using BookStore.Application.Features.Books.Commands.CreateBook;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests;

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<ICategoryRepository> _categoryRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly CreateBookCommandHandler _handler;

    private readonly Faker _faker = new();

    public CreateBookCommandHandlerTests()
    {
        _handler = new CreateBookCommandHandler(
            _bookRepository.Object,
            _categoryRepository.Object,
            _unitOfWork.Object);
    }

    private CreateBookCommand GenerateCommand()
    {
        return new CreateBookCommand(
            _faker.Commerce.ProductName(),
            _faker.Person.FullName,
            _faker.Lorem.Sentence(),
            "9780306406157",
            _faker.Random.Decimal(10, 100),
            _faker.Random.Int(1, 100),
            _faker.Random.Int(50, 500),
            _faker.Internet.Url(),
            _faker.Company.CompanyName(),
            DateOnly.FromDateTime(_faker.Date.Past()),
            Domain.Enums.BookFormat.Paperback,
            "PT-BR",
            Guid.NewGuid()
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldSucceed()
    {
        var command = GenerateCommand();

        var categoryResult = Category.Create("Tech", "tech");
        categoryResult.IsSuccess.Should().BeTrue();

        var category = categoryResult.Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _bookRepository
            .Setup(x => x.IsbnExistsAsync(It.IsAny<Domain.ValueObjects.Isbn>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _bookRepository.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldFail()
    {
        var command = GenerateCommand();

        _categoryRepository
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _bookRepository.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidIsbn_ShouldFail()
    {
        var command = GenerateCommand() with { Isbn = "123" };

        var categoryResult = Category.Create("Tech", "tech");
        categoryResult.IsSuccess.Should().BeTrue();

        var category = categoryResult.Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenIsbnAlreadyExists_ShouldFail()
    {
        var command = GenerateCommand();

        var categoryResult = Category.Create("Tech", "tech");
        categoryResult.IsSuccess.Should().BeTrue();

        var category = categoryResult.Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _bookRepository
            .Setup(x => x.IsbnExistsAsync(It.IsAny<Domain.ValueObjects.Isbn>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenDomainFails_ShouldFail()
    {
        var command = GenerateCommand() with { Title = "" };

        var categoryResult = Category.Create("Tech", "tech");
        categoryResult.IsSuccess.Should().BeTrue();

        var category = categoryResult.Value;

        _categoryRepository
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _bookRepository
            .Setup(x => x.IsbnExistsAsync(It.IsAny<Domain.ValueObjects.Isbn>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
