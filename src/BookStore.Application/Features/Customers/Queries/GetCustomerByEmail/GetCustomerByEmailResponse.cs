namespace BookStore.Application.Features.Customers.Queries.GetCustomerByEmail;

public sealed record GetCustomerByEmailResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Phone,
    string Role,
    bool IsActive);
