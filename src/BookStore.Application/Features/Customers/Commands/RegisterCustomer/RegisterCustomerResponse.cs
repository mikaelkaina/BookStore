namespace BookStore.Application.Features.Customers.Commands.RegisterCustomer;

public sealed record RegisterCustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string? Phone,
    string Document,
    DateOnly? BirthDate,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);