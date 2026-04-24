using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using MediatR;

namespace BookStore.Application.Features.Customers.Commands.RegisterCustomer;

public sealed class RegisterCustomerCommandHandler
    : IRequestHandler<RegisterCustomerCommand, Result<RegisterCustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterCustomerResponse>> Handle(
        RegisterCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<RegisterCustomerResponse>(emailResult.Error);

        var emailExists = await _customerRepository.EmailExistsAsync(
            emailResult.Value, cancellationToken: cancellationToken);

        if (emailExists)
            return Result.Failure<RegisterCustomerResponse>(
                CustomerErrors.EmailAlreadyExixts(request.Email));

        var customerResult = Customer.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Document,
            request.BirthDate);

        if (customerResult.IsFailure)
            return Result.Failure<RegisterCustomerResponse>(customerResult.Error);

        await _customerRepository.AddAsync(customerResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customerResult.Value.ToRegisterResponse());
    }
}
