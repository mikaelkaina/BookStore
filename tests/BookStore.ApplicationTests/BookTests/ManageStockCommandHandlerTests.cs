using BookStore.Application.Features.Books.Commands.ManageStock;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Events;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.BookTests;

public class ManageStockCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly AddStockCommandHandler _handler;
    private readonly DecrementStockCommandHandler _handler2;

    public ManageStockCommandHandlerTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new AddStockCommandHandler(
            _unitOfWork.Object,
            _bookRepository.Object
        );

        _handler2 = new DecrementStockCommandHandler(
            _unitOfWork.Object,
            _bookRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidQuantity_ShouldSucceed()
    {
        var command = new AddStockCommand(Guid.NewGuid(), 10);

        var book = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            5,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        book.StockQuantity.Should().Be(15);

        _bookRepository.Verify(x => x.UpdateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookNotFound_ShouldFail()
    {
        var command = new AddStockCommand(Guid.NewGuid(), 10);

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidQuantity_ShouldFail()
    {
        var command = new AddStockCommand(Guid.NewGuid(), 0);

        var validBook = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            5,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validBook);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    //

    [Fact]
    public async Task Handle_WithValidQuantity_ShouldSucceedd()
    {
        var command = new DecrementStockCommand(Guid.NewGuid(), 3);

        var book = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            10,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler2.Handle(command, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        book.StockQuantity.Should().Be(7);
    }

    [Fact]
    public async Task Handle_WithInsufficientStock_ShouldFail()
    {
        var command = new DecrementStockCommand(Guid.NewGuid(), 20);

        var book = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            5,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler2.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenStockBecomesZero_ShouldRaiseEvent()
    {
        var command = new DecrementStockCommand(Guid.NewGuid(), 5);

        var book = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50,
            5,
            200,
            null,
            "Prentice Hall",
            new DateOnly(2000, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        ).Value;

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler2.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        book.StockQuantity.Should().Be(0);
        book.DomainEvents.Should().Contain(e => e is BookOutOfStockEvent);
    }
}
