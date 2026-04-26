using BookStore.Application.Features.Orders.Commands.ApplyDiscount;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Commands;

public class ApplyDiscountCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly ApplyDiscountCommandHandler _handler;

    public ApplyDiscountCommandHandlerTests()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();

        _handler = new ApplyDiscountCommandHandler(
            _unitOfWork.Object,
            _orderRepository.Object
        );
    }

    [Fact]
    public async Task Handle_WithValidDiscount_ShouldSucceed()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .Build();

        var command = new ApplyDiscountCommand(order.Id, 10);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

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
        var command = new ApplyDiscountCommand(Guid.NewGuid(), 10);

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNegativeDiscount_ShouldFail()
    {
        var order = new OrderBuilder().Build();

        var command = new ApplyDiscountCommand(order.Id, -10);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithDiscountGreaterThanSubtotal_ShouldFail()
    {
        var order = new OrderBuilder()
            .WithItem(50)
            .Build();

        var command = new ApplyDiscountCommand(order.Id, 100);

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
}
