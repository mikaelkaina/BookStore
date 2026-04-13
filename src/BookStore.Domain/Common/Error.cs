namespace BookStore.Domain.Common;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");

    public static Error NotFound(string entity, Guid id) =>
        new($"{entity}.NotFound", $"{entity} with id '{id}' was not found.");

    public static Error Validation(string field, string message) =>
        new($"Validation.{field}", message);

    public static Error Conflict(string code, string description) =>
        new(code, description);
}