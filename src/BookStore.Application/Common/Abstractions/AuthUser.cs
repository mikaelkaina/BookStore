namespace BookStore.Application.Common.Abstractions;

public sealed record AuthUser(
    string Id, string Email, string FirstName, string LastName,
    Guid? CustomerId, IReadOnlyList<string> Roles,
    string? RefreshToken, DateTime? RefreshTokenExpiresAt);
