namespace BookStore.Application.Common.Abstractions;

public sealed record NewUser(
    string Email, string FirstName, string LastName,
    Guid CustomerId, string RefreshToken, DateTime RefreshTokenExpiresAt);
