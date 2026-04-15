using BookStore.Domain.Common;

namespace BookStore.Domain.ValueObjects;

public sealed class Isbn : ValueObject
{
    public string Value { get; }

    private Isbn(string value) => Value = value;

    public static Result<Isbn> Create(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return Result.Failure<Isbn>(Error.Validation(nameof(Isbn), "ISBN is required"));

        var clean = isbn.Replace("-", "").Replace(" ", "");
        if (clean.Length == 13 && IsValidIsbn13(clean))
            return Result.Success(new Isbn(clean));

        if (clean.Length == 10 && IsValidIsbn10(clean))
            return Result.Success(new Isbn(clean));

        return Result.Failure<Isbn>(Error.Validation(nameof(Isbn), "ISBN must be a valid ISBN-10 or ISBN-13."));
    }

    private static bool IsValidIsbn13(string isbn)
    {
        if (!isbn.All(char.IsDigit)) return false;
        var sum = 0;
        for (var  i = 0;  i < 12;  i++)
            sum += (isbn[i] - '0') * (i % 2 == 0 ? 1 : 3);
        var checkDigit = (10 - sum % 10) % 10;
        return checkDigit == isbn[12] - '0';
    }

    private static bool IsValidIsbn10(string isbn)
    {
        if (!isbn[..9].All(char.IsDigit)) return false;
        if (!char.IsDigit(isbn[9]) && isbn[9] != 'X' && isbn[9] != 'x') return false;
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (isbn[i] - '0') * (10 - i);
        var last = isbn[9] == 'X' || isbn[9] == 'x' ? 10 : isbn[9] - '0';
        sum += last;
        return sum % 11 == 0;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
