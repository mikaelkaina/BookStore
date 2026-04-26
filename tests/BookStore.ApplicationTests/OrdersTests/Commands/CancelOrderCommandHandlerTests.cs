using BookStore.Application.Features.Orders.Commands.CancelOrder;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class CancelOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new CancelOrderCommandHandler(
            _orderRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidOrder_ShouldCancelSuccessfully()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .Build();

        var command = new CancelOrderCommand(order.Id, "Cliente desistiu");

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancelledAt.Should().NotBeNull();
        order.Notes.Should().Be("Cliente desistiu");

        _orderRepository.Verify(
            x => x.UpdateAsync(order, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldFail()
    {
        var command = new CancelOrderCommand(Guid.NewGuid(), "Motivo");

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyDelivered_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Delivered)
            .Build();

        var command = new CancelOrderCommand(order.Id, "Motivo");

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _orderRepository.Verify(
            x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderAlreadyReturned_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Returned)
            .Build();

        var command = new CancelOrderCommand(order.Id, "Motivo");

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
