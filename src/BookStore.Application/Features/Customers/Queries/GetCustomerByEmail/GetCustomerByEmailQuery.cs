using BookStore.Application.Common.Messaging;
using BookStore.Domain.Common;

namespace BookStore.Application.Features.Customers.Queries.GetCustomerByEmail;

public sealed record GetCustomerByEmailQuery(string Email)
    : IQuery<Result<GetCustomerByEmailResponse>>;
