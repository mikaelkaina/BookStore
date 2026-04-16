using BookStore.Domain.Entities;
using BookStore.Domain.ValueObjects;

namespace BookStore.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(Email email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default);
}
