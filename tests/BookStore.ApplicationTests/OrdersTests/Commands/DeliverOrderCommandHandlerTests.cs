using BookStore.Application.Features.Orders.Commands.DeliverOrder;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class DeliverOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly DeliverOrderCommandHandler _handler;

    public DeliverOrderCommandHandlerTests()
    {
        _handler = new DeliverOrderCommandHandler(
            _orderRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidStatus_ShouldSucceed()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Shipped)
            .Build();

        var command = new DeliverOrderCommand(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(x => x.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        order.Status.Should().Be(OrderStatus.Delivered);
        order.DeliveredAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldFail()
    {
        var command = new DeliverOrderCommand(Guid.NewGuid());

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenStatusIsNotShipped_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Pending)
            .Build();

        var command = new DeliverOrderCommand(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _orderRepository.Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}