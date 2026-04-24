namespace BookStore.Application.Features.Customers.Commands.UpdateCustomerProfile;

public sealed record UpdateCustomerProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string? Phone,
    DateTime? UpdatedAt
);