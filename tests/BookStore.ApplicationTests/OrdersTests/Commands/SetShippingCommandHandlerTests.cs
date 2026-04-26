using BookStore.Application.Features.Orders.Commands.SetShipping;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class SetShippingCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly SetShippingCommandHandler _handler;

    public SetShippingCommandHandlerTests()
    {
        _handler = new SetShippingCommandHandler(
            _orderRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidShipping_ShouldSucceed()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .Build();

        var command = new SetShippingCommand(order.Id, 25);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(x => x.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        order.ShippingCost.Amount.Should().Be(25);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldFail()
    {
        var command = new SetShippingCommand(Guid.NewGuid(), 20);

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenShippingCostIsNegative_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .Build();

        var command = new SetShippingCommand(order.Id, -10);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _orderRepository.Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
