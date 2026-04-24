using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Customers.Commands.UpdateCustomerProfile;

public sealed record UpdateCustomerProfileCommand(
    Guid CustomerId,
    string FirstName,
    string LastName,
    string? Phone) : ICommand<Result<UpdateCustomerProfileResponse>>;