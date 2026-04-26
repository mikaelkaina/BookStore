using BookStore.Application.Features.Carts.Commands.ClearCart;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CartsTests.Commands;

public class ClearCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ClearCartCommandHandler _handler;

    public ClearCartCommandHandlerTests()
    {
        _handler = new ClearCartCommandHandler(
            _cartRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithExistingCart_ShouldClearAndSucceed()
    {
        var cart = Cart.CreateForGuest("session-123");

        var book = CreateValidBook();

        var addResult = cart.AddItem(book, 1);
        addResult.IsSuccess.Should().BeTrue();

        var command = new ClearCartCommand(cart.Id);

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();

        _cartRepository.Verify(x => x.UpdateAsync(cart, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistingCart_ShouldReturnFailure()
    {
        var command = new ClearCartCommand(Guid.NewGuid());

        _cartRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _cartRepository.Verify(x => x.UpdateAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmptyCart_ShouldStillSucceed()
    {
        var cart = Cart.CreateForGuest("session-123");

        var command = new ClearCartCommand(cart.Id);

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cart.Items.Should().BeEmpty();
    }

    private static Book CreateValidBook()
    {
        var result = Book.Create(
            "Clean Code",
            "Robert C. Martin",
            null,
            "9780132350884",
            10,
            1,
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