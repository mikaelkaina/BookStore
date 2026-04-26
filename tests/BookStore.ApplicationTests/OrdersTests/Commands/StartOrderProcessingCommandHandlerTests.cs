using BookStore.Application.Features.Orders.Commands.StartOrderProcessing;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class StartOrderProcessingCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly StartOrderProcessingCommandHandler _handler;

    public StartOrderProcessingCommandHandlerTests()
    {
        _handler = new StartOrderProcessingCommandHandler(
            _orderRepository.Object,
            _unitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidStatus_ShouldSucceed()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.PaymentConfirmed)
            .Build();

        var command = new StartOrderProcessingCommand(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _orderRepository.Verify(x => x.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        order.Status.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldFail()
    {
        var command = new StartOrderProcessingCommand(Guid.NewGuid());

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenStatusIsNotPaymentConfirmed_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .WithStatus(OrderStatus.Pending)
            .Build();

        var command = new StartOrderProcessingCommand(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();

        _orderRepository.Verify(x => x.UpdateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}