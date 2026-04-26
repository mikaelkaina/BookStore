using BookStore.Application.Features.Orders.Queries.GetOrdersByCustomer;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Queries;

public class GetOrdersByCustomerQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();

    private readonly GetOrdersByCustomerQueryHandler _handler;

    public GetOrdersByCustomerQueryHandlerTests()
    {
        _handler = new GetOrdersByCustomerQueryHandler(
            _orderRepository.Object,
            _customerRepository.Object);
    }

    [Fact]
    public async Task Handle_WithExistingCustomer_ShouldReturnOrders()
    {
        var customer = new CustomerBuilder().Build();

        var orders = new List<Order>
        {
            new OrderBuilder().WithItem(100).Build(),
            new OrderBuilder().WithItem(200).Build()
        };

        var query = new GetOrdersByCustomerQuery(customer.Id);

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _orderRepository
            .Setup(x => x.GetByCustomerIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var response = result.Value.ToList();

        response.Should().HaveCount(2);
        response.First().Id.Should().Be(orders.First().Id);
    }

    [Fact]
    public async Task Handle_WhenCustomerNotFound_ShouldFail()
    {
        var query = new GetOrdersByCustomerQuery(Guid.NewGuid());

        _customerRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNoOrders_ShouldReturnEmptyList()
    {
        var customer = new CustomerBuilder().Build();

        var query = new GetOrdersByCustomerQuery(customer.Id);

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _orderRepository
            .Setup(x => x.GetByCustomerIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldMapFieldsCorrectly()
    {
        var customer = new CustomerBuilder().Build();

        var order = new OrderBuilder()
            .WithItem(150)
            .Build();

        var query = new GetOrdersByCustomerQuery(customer.Id);

        _customerRepository
            .Setup(x => x.GetByIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _orderRepository
            .Setup(x => x.GetByCustomerIdAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Order> { order });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var response = result.Value.First();

        response.Id.Should().Be(order.Id);
        response.OrderNumber.Should().Be(order.OrderNumber);
        response.Status.Should().Be(order.Status.ToString());
        response.Total.Should().Be(order.Total.Amount);
        response.Currency.Should().Be(order.Total.Currency);
        response.ItemCount.Should().Be(order.Items.Count);
    }
}