using BookStore.Application.Features.Carts.Commands.AddItemToCart;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CartsTests.Commands;

public class AddItemToCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository = new();
    private readonly Mock<IBookRepository> _bookRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly AddItemToCartCommandHandler _handler;

    public AddItemToCartCommandHandlerTests()
    {
        _handler = new AddItemToCartCommandHandler(
            _cartRepository.Object,
            _bookRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WhenBookNotFound_ShouldFail()
    {
        var command = new AddItemToCartCommand(null, "session", Guid.NewGuid(), 1);

        _bookRepository
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithCustomerExistingCart_ShouldUpdateCart()
    {
        var customerId = Guid.NewGuid();

        var book = CreateValidBook();
        var cart = Cart.CreateForCustomer(customerId);

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _cartRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var command = new AddItemToCartCommand(customerId, null, book.Id, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _cartRepository.Verify(x => x.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCustomerWithoutCart_ShouldCreateCart()
    {
        var customerId = Guid.NewGuid();
        var book = CreateValidBook();

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _cartRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var command = new AddItemToCartCommand(customerId, null, book.Id, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _cartRepository.Verify(x => x.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithGuestExistingCart_ShouldUpdateCart()
    {
        var sessionId = "session-123";
        var book = CreateValidBook();
        var cart = Cart.CreateForGuest(sessionId);

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _cartRepository
            .Setup(x => x.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var command = new AddItemToCartCommand(null, sessionId, book.Id, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _cartRepository.Verify(x => x.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithGuestWithoutCart_ShouldCreateCart()
    {
        var sessionId = "session-123";
        var book = CreateValidBook();

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _cartRepository
            .Setup(x => x.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var command = new AddItemToCartCommand(null, sessionId, book.Id, 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _cartRepository.Verify(x => x.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAddItemFails_ShouldReturnFailure()
    {
        var sessionId = "session-123";
        var book = CreateBookWithoutStock();

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _cartRepository
            .Setup(x => x.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var command = new AddItemToCartCommand(null, sessionId, book.Id, 10);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }


    private Book CreateValidBook(decimal price = 10, int stock = 10)
    {
        var result = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            price,
            stock,
            100,
            null,
            "Desc",
            new DateOnly(2020, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        result.IsSuccess.Should().BeTrue("CreateValidBook deve sempre gerar um livro válido");

        return result.Value;
    }

    private static Book CreateBookWithoutStock()
    {
        var result = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            10,
            10, 
            100,
            null,
            "Desc",
            new DateOnly(2020, 1, 1),
            Domain.Enums.BookFormat.Paperback,
            "EN",
            Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();

        var book = result.Value;

        var decrementResult = book.DecrementStock(10);
        decrementResult.IsSuccess.Should().BeTrue();

        return book;
    }
}