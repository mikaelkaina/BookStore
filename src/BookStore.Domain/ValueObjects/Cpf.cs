using BookStore.Domain.Common;

namespace BookStore.Domain.ValueObjects;

public sealed class Cpf : ValueObject
{
    public string Value { get; }

    private Cpf(string value) => Value = value;

    public static Result<Cpf> Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return Result.Failure<Cpf>(
                Error.Validation(nameof(Cpf), "CPF is required."));

        var clean = Normalize(cpf);

        if (!IsValid(clean))
            return Result.Failure<Cpf>(
                Error.Validation(nameof(Cpf), "CPF is invalid."));

        return Result.Success(new Cpf(clean));
    }

    private static string Normalize(string cpf) =>
        cpf.Replace(".", "").Replace("-", "").Trim();

    private static bool IsValid(string clean)
    {
        if (clean.Length != 11 || !clean.All(char.IsDigit)) return false;
        if (clean.Distinct().Count() == 1) return false;

        var sum = 0;
        for (var i = 0; i < 9; i++) sum += (clean[i] - '0') * (10 - i);
        var r = sum % 11;
        var d1 = r < 2 ? 0 : 11 - r;

        sum = 0;
        for (var i = 0; i < 10; i++) sum += (clean[i] - '0') * (11 - i);
        r = sum % 11;
        var d2 = r < 2 ? 0 : 11 - r;

        return clean[9] - '0' == d1 && clean[10] - '0' == d2;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
