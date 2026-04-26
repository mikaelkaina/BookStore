using BookStore.Application.Features.Carts.Commands.UpdateItemQuantity;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CartsTests.Commands;

public class UpdateItemQuantityCommandHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository = new();
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly UpdateItemQuantityCommandHandler _handler;

    public UpdateItemQuantityCommandHandlerTests()
    {
        _handler = new UpdateItemQuantityCommandHandler(
            _cartRepository.Object,
            _bookRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidQuantity_ShouldUpdateItem()
    {
        var cart = Cart.CreateForGuest("session-123");
        var book = CreateValidBook();

        cart.AddItem(book, 1);

        var command = new UpdateItemQuantityCommand(cart.Id, book.Id, 5);

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cart.Items.First().Quantity.Should().Be(5);

        _cartRepository.Verify(x => x.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenQuantityIsZero_ShouldRemoveItem()
    {
        var cart = Cart.CreateForGuest("session-123");
        var book = CreateValidBook();

        var addResult = cart.AddItem(book, 2);
        addResult.IsSuccess.Should().BeTrue();

        var command = new UpdateItemQuantityCommand(cart.Id, book.Id, 0);

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenCartNotFound_ShouldFail()
    {
        var command = new UpdateItemQuantityCommand(Guid.NewGuid(), Guid.NewGuid(), 1);

        _cartRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenBookNotFound_ShouldFail()
    {
        var cart = Cart.CreateForGuest("session-123");

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var command = new UpdateItemQuantityCommand(cart.Id, Guid.NewGuid(), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldFail()
    {
        var cart = Cart.CreateForGuest("session-123");
        var book = CreateValidBook();

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var command = new UpdateItemQuantityCommand(cart.Id, book.Id, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenInsufficientStock_ShouldFail()
    {
        var cart = Cart.CreateForGuest("session-123");
        var book = CreateValidBook();

        cart.AddItem(book, 1);

        var command = new UpdateItemQuantityCommand(cart.Id, book.Id, 999);

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _bookRepository
            .Setup(x => x.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    private static Book CreateValidBook()
    {
        var result = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            50, 
            10, 
            100,
            null,
            "Desc",
            new DateOnly(2020, 1, 1),
            Domain.Enums.BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        result.IsSuccess.Should().BeTrue();

        var book = result.Value;

        book.Activate(); 

        return book;
    }
}