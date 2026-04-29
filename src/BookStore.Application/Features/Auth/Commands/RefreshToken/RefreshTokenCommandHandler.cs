using BookStore.Domain.Common;
using BookStore.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace BookStore.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
            return Result.Failure<AuthResponse>(
                new Error("Auth.InvalidToken", "Invalid access token."));

        var userId = principal.FindFirst(
            System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (userId is null)
            return Result.Failure<AuthResponse>(
                new Error("Auth.InvalidToken", "Invalid token claims."));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null ||
            user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiresAt < DateTime.UtcNow)
            return Result.Failure<AuthResponse>(
                new Error("Auth.InvalidRefreshToken",
                    "Refresh token is invalid or expired."));

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthResponse(
            newAccessToken,
            newRefreshToken,
            DateTime.UtcNow.AddMinutes(15),
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.CustomerId,
            roles));
    }
}