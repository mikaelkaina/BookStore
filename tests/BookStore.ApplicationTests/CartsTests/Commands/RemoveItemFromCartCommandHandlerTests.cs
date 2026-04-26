using BookStore.Application.Features.Carts.Commands.RemoveItemFromCart;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CartsTests.Commands;

public class RemoveItemFromCartCommandHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly RemoveItemFromCartCommandHandler _handler;

    public RemoveItemFromCartCommandHandlerTests()
    {
        _handler = new RemoveItemFromCartCommandHandler(
            _cartRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldRemoveItem()
    {
        var cart = Cart.CreateForGuest("session-123");

        var book = CreateValidBook();

        cart.AddItem(book, 1);

        var command = new RemoveItemFromCartCommand(cart.Id, book.Id);

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
    public async Task Handle_WhenCartNotFound_ShouldFail()
    {
        var command = new RemoveItemFromCartCommand(Guid.NewGuid(), Guid.NewGuid());

        _cartRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenItemNotFound_ShouldFail()
    {
        var cart = Cart.CreateForGuest("session-123");

        var command = new RemoveItemFromCartCommand(cart.Id, Guid.NewGuid());

        _cartRepository
            .Setup(x => x.GetByIdAsync(cart.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

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
            100,
            1,
            200,
            null,
            "Desc",
            new DateOnly(2020, 1, 1),
            Domain.Enums.BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }
}
