using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.ApplicationTests.Builders;

public class OrderBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private Address _shippingAddress = BuildDefaultAddress();
    private OrderStatus _status = OrderStatus.Pending;

    private readonly List<(int quantity, decimal price)> _items = [];

    public OrderBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public OrderBuilder WithShippingAddress(Address address)
    {
        _shippingAddress = address;
        return this;
    }

    public OrderBuilder WithStatus(OrderStatus status)
    {
        _status = status;
        return this;
    }

    public OrderBuilder WithItem(decimal price, int quantity = 1)
    {
        _items.Add((quantity, price));
        return this;
    }

    public Order Build()
    {
        var result = Order.Create(_customerId, _shippingAddress);
        result.IsSuccess.Should().BeTrue();

        var order = result.Value;

        foreach (var (quantity, price) in _items)
        {
            var book = CreateValidBook(price, quantity);

            var addItemResult = order.AddItem(book, quantity);
            addItemResult.IsSuccess.Should().BeTrue();
        }

        // força status
        typeof(Order)
            .GetProperty("Status")!
            .SetValue(order, _status);

        return order;
    }

    private static Address BuildDefaultAddress()
    {
        var result = Address.Create(
            "Rua das Flores", "123", null,
            "Centro", "São Paulo", "SP", "01310100");

        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    private static Book CreateValidBook(decimal price, int stock)
    {
        var result = Book.Create(
            "Test Book",
            "Test Author",
            null,
            "9780306406157",
            price,
            stock,
            100,
            null,
            "Test Publisher",
            new DateOnly(2020, 1, 1),
            BookFormat.Paperback,
            "EN",
            Guid.NewGuid()
        );

        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }
}