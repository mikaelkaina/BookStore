namespace BookStore.Application.Common.Abstractions.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(AuthUser user);
    string GenerateRefreshToken();
    string? GetUserIdFromExpiredToken(string accessToken);
}
