using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    public CustomerRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
        => await _context.Customers.AddAsync(entity, cancellationToken);

    public Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        _context.Customers.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(Email email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default)
        => await _context.Customers
        .AnyAsync(c =>
            c.Email.Value == email.Value &&
            (excludeCustomerId == null || c.Id != excludeCustomerId.Value),
            cancellationToken);

    public Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => _context.Customers.FirstOrDefaultAsync(c => c.Email.Value == email.Value, cancellationToken);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(entity);
        return Task.CompletedTask;
    }
}
