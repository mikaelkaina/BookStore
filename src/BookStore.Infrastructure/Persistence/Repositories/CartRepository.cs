using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    public CartRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(Cart entity, CancellationToken cancellationToken = default)
        => await _context.Carts.AddAsync(entity, cancellationToken);
    public Task DeleteAsync(Cart entity, CancellationToken cancellationToken = default)
    {
        _context.Carts.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _context.Carts
        .AsNoTracking()
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Carts
        .AsNoTracking()
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default)
        => await _context.Carts
        .AsNoTracking()
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.SessionId == sessionId, cancellationToken);

    public Task UpdateAsync(Cart entity, CancellationToken cancellationToken = default)
    {
        _context.Carts.Update(entity);
        return Task.CompletedTask;
    }
}
