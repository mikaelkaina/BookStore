using BookStore.Application.Common.Abstractions;
using BookStore.Application.Common.Abstractions.Interfaces;
using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Interfaces.Repositories;
using MediatR;

namespace BookStore.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IIdentityService identityService,
        ITokenService tokenService,
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _identityService.EmailExistsAsync(request.Email))
            return Result.Failure<AuthResponse>(new Error("Auth.EmailAlreadyExists",
                $"Email '{request.Email}' is already registered."));

        var customerResult = Customer.Create(request.FirstName, request.LastName,
            request.Email, request.Phone, request.Document, request.BirthDate);
        if (customerResult.IsFailure)
            return Result.Failure<AuthResponse>(customerResult.Error);

        _customerRepository.Add(customerResult.Value, cancellationToken);

        var refreshToken = _tokenService.GenerateRefreshToken();

        var userResult = await _identityService.CreateCustomerUserAsync(
            new NewUser(request.Email, request.FirstName, request.LastName,
                customerResult.Value.Id, refreshToken, DateTime.UtcNow.AddDays(7)),
            request.Password);
        if (userResult.IsFailure)
            return Result.Failure<AuthResponse>(userResult.Error);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = userResult.Value;
        return Result.Success(AuthResponse.From(user, _tokenService.GenerateAccessToken(user), refreshToken));
    }
}
