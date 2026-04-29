using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Document,
    string? Phone = null,
    DateOnly? BirthDate = null
) : ICommand<Result<AuthResponse>>;
