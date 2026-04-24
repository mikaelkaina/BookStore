using BookStore.Application.Features.Customers.Commands.RegisterCustomer;
using BookStore.Application.Features.Customers.Commands.UpdateCustomerProfile;
using BookStore.Domain.Entities;

namespace BookStore.Application.Features.Customers;

public static class CustomerMappingExtensions
{
    public static RegisterCustomerResponse ToRegisterResponse(this Customer customer) =>
        new(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.FullName,
            customer.Email.Value,
            customer.Phone,
            customer.Document.Value,
            customer.BirthDate,
            customer.Role.ToString(),
            customer.IsActive,
            customer.CreatedAt
        );

    public static UpdateCustomerProfileResponse ToUpdateProfileResponse(this Customer customer) =>
        new(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.FullName,
            customer.Phone,
            customer.UpdatedAt);
}
