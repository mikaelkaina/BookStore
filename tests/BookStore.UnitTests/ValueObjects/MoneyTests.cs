using BookStore.Domain.ValueObjects;
using FluentAssertions;

namespace BookStore.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldSucceed()
    {
        var result = Money.Create(99.90m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(99.90m);
        result.Value.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldSucceed()
    {
        var result = Money.Create(0m);
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        var result = Money.Create(-1m);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Amount");
    }

    [Fact]
    public void Create_WithInvalidCurrency_ShouldFail()
    {
        var result = Money.Create(10m, "XX");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Currency");
    }

    [Fact]
    public void Create_ShouldNormalizeCurrencyToUppercase()
    {
        var result = Money.Create(10m, "brl");

        result.IsSuccess.Should().BeTrue();
        result.Value.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_SameCurrency_ShouldReturnSum()
    {
        var a = Money.Create(50m).Value;
        var b = Money.Create(30m).Value;

        var sum = a.Add(b);

        sum.Amount.Should().Be(80m);
        sum.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_DifferentCurrency_ShouldThrow()
    {
        var brl = Money.Create(50m, "BRL").Value;
        var usd = Money.Create(50m, "USD").Value;

        var act = () => brl.Add(usd);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Subtract_SameCurrency_ShouldReturnDifference()
    {
        var a = Money.Create(100m).Value;
        var b = Money.Create(40m).Value;

        var diff = a.Subtract(b);
        diff.Amount.Should().Be(60m);
    }

    [Fact]
    public void Multiply_ShouldReturnCorrectTotal()
    {
        var price = Money.Create(25m).Value;

        var total = price.Multiply(4);
        total.Amount.Should().Be(100m);
    }

    [Fact]
    public void IsGreaterThan_ShouldReturnCorrectComparison()
    {
        var high = Money.Create(100m).Value;
        var low = Money.Create(50m).Value;

        high.IsGreaterThan(low).Should().BeTrue();
        low.IsGreaterThan(high).Should().BeFalse();
    }

    [Fact]
    public void Zero_ShouldReturnMoneyWithZeroAmount()
    {
        var zero = Money.Zero();

        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Equality_SameAmountAndCurrency_ShouldBeEqual()
    {
        var a = Money.Create(100m, "BRL").Value;
        var b = Money.Create(100m, "BRL").Value;

        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentAmount_ShouldNotBeEqual()
    {
        var a = Money.Create(100m).Value;
        var b = Money.Create(200m).Value;

        (a != b).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var money = Money.Create(99.99m).Value;
        money.ToString().Should().Be("99.99 BRL");
    }
}
