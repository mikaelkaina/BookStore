using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtService _jwtService;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork, JwtService jwtService)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result.Failure<AuthResponse>(
                new Error("Auth.EmailAlreadyExists",
                    $"Email '{request.Email}' is already registered."));

        var customerResult = Customer.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Document,
            request.BirthDate);

        if (customerResult.IsFailure)
            return Result.Failure<AuthResponse>(customerResult.Error);

        await _customerRepository.AddAsync(customerResult.Value, cancellationToken);

        var refreshToken = _jwtService.GenerateRefreshToken();

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CustomerId = customerResult.Value.Id,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
            return Result.Failure<AuthResponse>(
                new Error("Auth.IdentityError", errors));
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);

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
}
