using BookStore.Application.Features.Orders.Queries.GetOrderById;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Queries;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();

    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _handler = new GetOrderByIdQueryHandler(_orderRepository.Object);
    }

    [Fact]
    public async Task Handle_WithExistingOrder_ShouldReturnSuccess()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .Build();

        var query = new GetOrderByIdQuery(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var response = result.Value;

        response.Id.Should().Be(order.Id);
        response.CustomerId.Should().Be(order.CustomerId);
        response.Status.Should().Be(order.Status.ToString());
        response.SubTotal.Should().Be(order.SubTotal.Amount);
        response.Total.Should().Be(order.Total.Amount);

        response.Items.Should().NotBeEmpty();
        response.ShippingAddress.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldFail()
    {
        var query = new GetOrderByIdQuery(Guid.NewGuid());

        _orderRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldMapItemsCorrectly()
    {
        var order = new OrderBuilder()
            .WithItem(100)
            .Build();

        var query = new GetOrderByIdQuery(order.Id);

        _orderRepository
            .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var item = result.Value.Items.First();

        item.BookId.Should().Be(order.Items.First().BookId);
        item.Quantity.Should().Be(order.Items.First().Quantity);
        item.UnitPrice.Should().Be(order.Items.First().UnitPrice.Amount);
    }
}
