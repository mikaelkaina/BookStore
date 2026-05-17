using BookStore.Domain.Common;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtService _jwtService;
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        JwtService jwtService,
        ICartRepository cartRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<AuthResponse>(
                new Error("Auth.InvalidCredentials", "Email or password is incorrect."));

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Result.Failure<AuthResponse>(
                new Error("Auth.InvalidCredentials", "Email or password is incorrect."));

        if (!string.IsNullOrEmpty(request.GuestSessionId) && user.CustomerId.HasValue)
            await MergeGuestCartAsync(request.GuestSessionId, user.CustomerId.Value, cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Result.Success(new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(15),
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.CustomerId,
            roles));
    }

    private async Task MergeGuestCartAsync(string sessionId, Guid customerId, CancellationToken cancellationToken)
    {
        var guestCart = await _cartRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        if(guestCart is null || !guestCart.Items.Any()) return;

        var customerCart = await _cartRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        if (customerCart is null)
        {
            guestCart.AssignToCustomer(customerId);
            await _cartRepository.UpdateAsync(guestCart, cancellationToken);
        }
        else
        {
            foreach(var guestItem in guestCart.Items)
                customerCart.MergerItem(guestItem);
            
            await _cartRepository.UpdateAsync(customerCart, cancellationToken);
            await _cartRepository.DeleteAsync(guestCart, cancellationToken);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}