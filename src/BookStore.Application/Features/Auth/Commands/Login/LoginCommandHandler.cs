using BookStore.Application.Common.Abstractions.Interfaces;
using BookStore.Domain.Common;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Interfaces.Repositories;
using MediatR;

namespace BookStore.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService,
        ICartRepository cartRepository,
        IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (user is null)
            return Result.Failure<AuthResponse>(Error.InvalidCredentials());

        if (!string.IsNullOrEmpty(request.GuestSessionId) && user.CustomerId.HasValue)
            await MergeGuestCartAsync(request.GuestSessionId, user.CustomerId.Value, cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        await _identityService.SetRefreshTokenAsync(user.Id, refreshToken, DateTime.UtcNow.AddDays(7));

        return Result.Success(AuthResponse.From(user, accessToken, refreshToken));
    }

    private async Task MergeGuestCartAsync(
       string sessionId,
       Guid customerId,
       CancellationToken cancellationToken)
    {
        var guestCart = await _cartRepository.GetBySessionIdNoTrackingAsync(sessionId, cancellationToken);
        if (guestCart is null || !guestCart.Items.Any()) return;

        var customerCart = await _cartRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        if (customerCart is null)
        {
            var trackedGuestCart = await _cartRepository.GetBySessionIdAsync(sessionId, cancellationToken);
            trackedGuestCart!.AssignToCustomer(customerId);
            await _cartRepository.UpdateAsync(trackedGuestCart, cancellationToken);
        }
        else
        {
            foreach (var guestItem in guestCart.Items)
                customerCart.MergeItem(guestItem);

            await _cartRepository.UpdateAsync(customerCart, cancellationToken);
            await _cartRepository.DeleteByIdAsync(guestCart.Id, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}