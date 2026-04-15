using BookStore.Domain.Common;
using System.Text.RegularExpressions;

namespace BookStore.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; }
    
    private Email(string value) => Value = value;

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>(Error.Validation(nameof(Email), "Email is required"));

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (!EmailRegex.IsMatch(normalizedEmail))
            return Result.Failure<Email>(Error.Validation(nameof(Email), "Email format is invalid"));

        return Result.Success(new Email(normalizedEmail));
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
