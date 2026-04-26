using BookStore.Application.Features.Orders.Queries.GetOrdersPaged;
using BookStore.ApplicationTests.Builders;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BookStore.ApplicationTests.OrdersTests.Queries;

public class GetOrdersPagedQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();

    private readonly GetOrdersPagedQueryHandler _handler;

    public GetOrdersPagedQueryHandlerTests()
    {
        _handler = new GetOrdersPagedQueryHandler(_orderRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnPagedResult()
    {
        var orders = new List<Order>
        {
            new OrderBuilder().WithItem(100).Build(),
            new OrderBuilder().WithItem(200).Build()
        };

        var query = new GetOrdersPagedQuery(
            CustomerId: null,
            Status: null,
            Page: 1,
            PageSize: 10
        );

        _orderRepository
            .Setup(x => x.GetPagedAsync(
                query.CustomerId,
                query.Status,
                query.Page,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((orders, totalCount: 2));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var response = result.Value;

        response.Items.Should().HaveCount(2);
        response.TotalCount.Should().Be(2);
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(10);
        response.TotalPages.Should().Be(1);
        response.HasNextPage.Should().BeFalse();
        response.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithInvalidPage_ShouldFail()
    {
        var query = new GetOrdersPagedQuery(
            null,
            null,
            Page: 0,
            PageSize: 10
        );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidPageSize_ShouldFail()
    {
        var query = new GetOrdersPagedQuery(
            null,
            null,
            Page: 1,
            PageSize: 0
        );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithPageSizeGreaterThan100_ShouldFail()
    {
        var query = new GetOrdersPagedQuery(
            null,
            null,
            Page: 1,
            PageSize: 101
        );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCalculatePaginationCorrectly()
    {
        var orders = new List<Order>
        {
            new OrderBuilder().WithItem(100).Build(),
            new OrderBuilder().WithItem(200).Build()
        };

        var query = new GetOrdersPagedQuery(
            null,
            null,
            Page: 2,
            PageSize: 1
        );

        _orderRepository
            .Setup(x => x.GetPagedAsync(
                query.CustomerId,
                query.Status,
                query.Page,
                query.PageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((orders, totalCount: 2));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var response = result.Value;

        response.TotalPages.Should().Be(2);
        response.HasNextPage.Should().BeFalse(); 
        response.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldMapFieldsCorrectly()
    {
        var order = new OrderBuilder()
            .WithItem(150)
            .Build();

        var query = new GetOrdersPagedQuery(null, null, 1, 10);

        _orderRepository
            .Setup(x => x.GetPagedAsync(
                It.IsAny<Guid?>(),
                It.IsAny<OrderStatus?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Order> { order }, totalCount: 1));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var item = result.Value.Items.First();

        item.Id.Should().Be(order.Id);
        item.OrderNumber.Should().Be(order.OrderNumber);
        item.CustomerId.Should().Be(order.CustomerId);
        item.Status.Should().Be(order.Status.ToString());
        item.Total.Should().Be(order.Total.Amount);
        item.Currency.Should().Be(order.Total.Currency);
        item.ItemCount.Should().Be(order.Items.Count);
    }
}