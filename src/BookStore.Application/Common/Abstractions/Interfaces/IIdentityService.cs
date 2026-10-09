using BookStore.Domain.Common;

namespace BookStore.Application.Common.Abstractions.Interfaces;

public interface IIdentityService
{
    Task<bool> EmailExistsAsync(string email);
    Task<AuthUser?> ValidateCredentialsAsync(string email, string password);
    Task<AuthUser?> FindByIdAsync(string userId);
    Task<Result<AuthUser>> CreateCustomerUserAsync(NewUser newUser, string password);
    Task SetRefreshTokenAsync(string userId, string refreshToken, DateTime expiresAt);
}
