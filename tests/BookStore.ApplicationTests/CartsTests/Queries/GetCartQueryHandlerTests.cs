using BookStore.Application.Features.Carts.Queries.GetCart;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.CartsTests.Queries;

public class GetCartQueryHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository = new();
    private readonly GetCartQueryHandler _handler;

    public GetCartQueryHandlerTests()
    {
        _handler = new GetCartQueryHandler(_cartRepository.Object);
    }

    [Fact]
    public async Task Handle_WithCustomerId_ShouldReturnCart()
    {
        var customerId = Guid.NewGuid();
        var cart = Cart.CreateForCustomer(customerId);

        var command = new GetCartQuery(customerId, null);

        _cartRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(cart.Id);
        result.Value.CustomerId.Should().Be(customerId);
    }

    [Fact]
    public async Task Handle_WithSessionId_ShouldReturnCart()
    {
        var sessionId = "session-123";
        var cart = Cart.CreateForGuest(sessionId);

        var command = new GetCartQuery(null, sessionId);

        _cartRepository
            .Setup(x => x.GetBySessionIdAsync(sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.SessionId.Should().Be(sessionId);
    }

    [Fact]
    public async Task Handle_WhenCartNotFound_ShouldReturnFailure()
    {
        var command = new GetCartQuery(Guid.NewGuid(), null);

        _cartRepository
            .Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenSessionIdNotFound_ShouldReturnFailure()
    {
        var command = new GetCartQuery(null, "session-123");

        _cartRepository
            .Setup(x => x.GetBySessionIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoCustomerIdAndNoSessionId_ShouldReturnFailure()
    {
        var command = new GetCartQuery(null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}