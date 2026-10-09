using BookStore.Application.Common.Abstractions;
using BookStore.Application.Common.Abstractions.Interfaces;
using BookStore.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    public IdentityService(UserManager<ApplicationUser> userManager)
        => _userManager = userManager;

    public async Task<bool> EmailExistsAsync(string email)
        => await _userManager.FindByEmailAsync(email) is not null;

    public async Task<AuthUser?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            return null;
        return await ToAuthUserAsync(user);
    }

    public async Task<AuthUser?> FindByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? null : await ToAuthUserAsync(user);
    }

    public async Task<Result<AuthUser>> CreateCustomerUserAsync(NewUser newUser, string password)
    {
        var user = new ApplicationUser
        {
            UserName = newUser.Email,
            Email = newUser.Email,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            CustomerId = newUser.CustomerId,
            RefreshToken = newUser.RefreshToken,
            RefreshTokenExpiresAt = newUser.RefreshTokenExpiresAt,
        };

        var created = await _userManager.CreateAsync(user, password);
        if (!created.Succeeded)
            return Result.Failure<AuthUser>(ToError(created));

        var inRole = await _userManager.AddToRoleAsync(user, "Customer");
        if (!inRole.Succeeded)
            return Result.Failure<AuthUser>(ToError(inRole));

        return Result.Success(await ToAuthUserAsync(user));
    }

    public async Task SetRefreshTokenAsync(string userId, string refreshToken, DateTime expiresAt)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new InvalidOperationException($"User '{userId}' not found.");
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = expiresAt;
        await _userManager.UpdateAsync(user);
    }

    private async Task<AuthUser> ToAuthUserAsync(ApplicationUser u)
        => new(u.Id, u.Email!, u.FirstName, u.LastName, u.CustomerId,
               (await _userManager.GetRolesAsync(u)).ToList(),
               u.RefreshToken, u.RefreshTokenExpiresAt);

    private static Error ToError(IdentityResult r)
        => new("Auth.IdentityError", string.Join(", ", r.Errors.Select(e => e.Description)));
}