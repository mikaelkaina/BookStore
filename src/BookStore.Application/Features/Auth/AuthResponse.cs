using BookStore.Application.Common.Abstractions;

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
    IEnumerable<string> Roles)
{
    public static AuthResponse From(AuthUser user, string accessToken, string refreshToken) => new(
        accessToken,
        refreshToken,
        DateTime.UtcNow.AddMinutes(15),
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.CustomerId,
        user.Roles);
}