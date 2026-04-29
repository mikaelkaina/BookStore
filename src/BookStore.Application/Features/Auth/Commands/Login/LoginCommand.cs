using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<Result<AuthResponse>>;
