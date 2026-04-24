using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Customers.Commands.RegisterCustomer;

public sealed record RegisterCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Document,
    DateOnly? BirthDate) : ICommand<Result<RegisterCustomerResponse>>;
