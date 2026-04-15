using BookStore.Domain.Entities;
using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.UnitTests.Builders;

public class OrderBuilder
{
    private Guid _customerId = Guid.NewGuid();
    private Address _shippingAddress = BuildDefaultAddress();

    public OrderBuilder WithCustomerId(Guid customerId) { _customerId = customerId; return this; }
    public OrderBuilder WithShippingAddress(Address address) { _shippingAddress = address; return this; }

    public Order Build()
    {
        var result = Order.Create(_customerId, _shippingAddress);
        result.IsSuccess.Should().BeTrue("OrderBuilder deve sempre gerar um pedido válido");
        return result.Value;
    }

    public static Address BuildDefaultAddress()
    {
        var result = Address.Create(
            "Rua das Flores", "123", null,
            "Centro", "São Paulo", "SP", "01310100");
        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }
}
