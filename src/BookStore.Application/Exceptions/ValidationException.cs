namespace BookStore.Application.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationError> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IEnumerable<ValidationError> Errors { get; }
}

public sealed record ValidationError(string Property, string Message);