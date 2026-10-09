using BookStore.Application.Common.Abstractions.Interfaces;
using BookStore.Domain.Common;
using MediatR;

namespace BookStore.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IIdentityService identityService, 
        ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);
        if (userId is null)
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidToken", "Invalid access token."));

        var user = await _identityService.FindByIdAsync(userId);
        if (user is null ||
            user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiresAt < DateTime.UtcNow)
            return Result.Failure<AuthResponse>(new Error("Auth.InvalidRefreshToken",
                "Refresh token is invalid or expired."));

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        await _identityService.SetRefreshTokenAsync(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7));

        return Result.Success(AuthResponse.From(user, newAccessToken, newRefreshToken));
    }
}