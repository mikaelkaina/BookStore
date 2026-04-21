using BookStore.Domain.Common;
using System.Globalization;

namespace BookStore.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;

    private Money() { }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            return Result.Failure<Money>(
                Error.Validation(nameof(Amount), "Amount cannot be negative."));

        if (string.IsNullOrWhiteSpace(currency))
            return Result.Failure<Money>(
                Error.Validation(nameof(Currency), "Currency cannot be empty."));

        var normalizedCurrency = currency.Trim().ToUpperInvariant();

        if (normalizedCurrency.Length != 3)
            return Result.Failure<Money>(
                Error.Validation(nameof(Currency), "Currency must have 3 characters."));

        return Result.Success(new Money(amount, normalizedCurrency));
    }

    public static Money Zero(string currency = "BRL") =>
        new Money(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(int quantity) =>
        new(Amount * quantity, Currency);

    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);
        return Amount > other.Amount;
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot operate on different currencies: {Currency} and {other.Currency}.");
    }

    public override string ToString() =>
        $"{Amount.ToString("F2", CultureInfo.InvariantCulture)} {Currency}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}