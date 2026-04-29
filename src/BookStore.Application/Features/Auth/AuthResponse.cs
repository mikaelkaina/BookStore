namespace BookStore.Application.Features.Auth;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    Guid? CustomerId,
    IEnumerable<string> Roles
);
