using BookStore.Application.Features.Orders.Commands.ConfirmOrderPayment;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class ConfirmOrderPaymentCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ConfirmOrderPaymentCommandHandler _handler;

    public ConfirmOrderPaymentCommandHandlerTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new ConfirmOrderPaymentCommandHandler(
            _unitOfWork.Object,
            _orderRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidOrder_ShouldSucceed()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Pending)
            .Build();

        var command = new ConfirmOrderPaymentCommand(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.PaymentConfirmed);

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
        var command = new ConfirmOrderPaymentCommand(Guid.NewGuid());

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenStatusIsNotPending_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Processing)
            .Build();

        var command = new ConfirmOrderPaymentCommand(order.Id);

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
    public async Task Handle_WhenOrderHasNoItems_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Pending)
            .Build(); 

        var command = new ConfirmOrderPaymentCommand(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
