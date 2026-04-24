namespace BookStore.Application.Features.Customers.Queries.GetCustomerById;

public sealed record GetCustomerByIdResponse(
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
    DateTime CreatedAt,
    DateTime? UpdatedAt
);