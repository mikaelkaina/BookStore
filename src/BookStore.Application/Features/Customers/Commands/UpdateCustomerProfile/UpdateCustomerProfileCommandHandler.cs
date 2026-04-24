using BookStore.Domain.Common;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using MediatR;

namespace BookStore.Application.Features.Customers.Commands.UpdateCustomerProfile;

public sealed class UpdateCustomerProfileCommandHandler
    : IRequestHandler<UpdateCustomerProfileCommand, Result<UpdateCustomerProfileResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerProfileCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateCustomerProfileResponse>> Handle(
        UpdateCustomerProfileCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result.Failure<UpdateCustomerProfileResponse>(
                CustomerErrors.NotFound(request.CustomerId));

        var result = customer.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.Phone);

        if (result.IsFailure)
            return Result.Failure<UpdateCustomerProfileResponse>(result.Error);

        await _customerRepository.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(customer.ToUpdateProfileResponse());
    }
}