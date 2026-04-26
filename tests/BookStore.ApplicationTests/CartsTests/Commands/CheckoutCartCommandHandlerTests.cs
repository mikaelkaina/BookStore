using BookStore.Application.Features.Carts.Commands.CheckoutCart;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;
namespace BookStore.ApplicationTests.CartsTests.Commands;

public class CheckoutCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CheckoutCartCommandHandler _handler;

    public CheckoutCartCommandHandlerTests()
    {
        _cartRepository = new Mock<ICartRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new CheckoutCartCommandHandler(
            _cartRepository.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithValidCart_ShouldSucceed()
    {
        var cart = CreateValidCartWithItem();

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var command = new CheckoutCartCommand(cart.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _cartRepository.Verify(x => x.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCartNotFound_ShouldFail()
    {
        _cartRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var command = new CheckoutCartCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCartIsEmpty_ShouldFail()
    {
        var cart = Cart.CreateForGuest("session-123");

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var command = new CheckoutCartCommand(cart.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenCartAlreadyCheckedOut_ShouldFail()
    {
        var cart = CreateValidCartWithItem();

        cart.Checkout();

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var command = new CheckoutCartCommand(cart.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }




    private static Cart CreateValidCartWithItem()
    {
        var cart = Cart.CreateForGuest("session-123");

        var book = CreateValidBook();

        var addResult = cart.AddItem(book, 1);
        addResult.IsSuccess.Should().BeTrue();

        return cart;
    }

    private static Book CreateValidBook()
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
        return result.Value;
    }
}