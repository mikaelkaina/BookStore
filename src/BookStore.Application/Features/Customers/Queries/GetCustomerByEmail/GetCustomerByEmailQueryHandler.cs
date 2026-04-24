using BookStore.Domain.Common;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using MediatR;

namespace BookStore.Application.Features.Customers.Queries.GetCustomerByEmail;

public sealed class GetCustomerByEmailQueryHandler
    : IRequestHandler<GetCustomerByEmailQuery, Result<GetCustomerByEmailResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByEmailQueryHandler(ICustomerRepository customerRepository)
        => _customerRepository = customerRepository;

    public async Task<Result<GetCustomerByEmailResponse>> Handle(
        GetCustomerByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return Result.Failure<GetCustomerByEmailResponse>(emailResult.Error);

        var customer = await _customerRepository.GetByEmailAsync(
            emailResult.Value, cancellationToken);

        if (customer is null)
            return Result.Failure<GetCustomerByEmailResponse>(
                new Error("Customer.NotFound",
                    $"Customer with email '{request.Email}' was not found."));

        return Result.Success(customer.ToGetByEmailResponse());
    }
}