using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(Order entity, CancellationToken cancellationToken = default)
        => await _context.Orders.AddAsync(entity, cancellationToken);

    public Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        _context.Orders.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        => await _context.Orders
        .Include(o => o.Items)
        .Where(o => o.CustomerId == customerId)
        .OrderByDescending(o => o.CreatedAt)
        .ToListAsync(cancellationToken);

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
         => await _context.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);

    public async Task<(IEnumerable<Order> Orders, int totalCount)> GetPagedAsync(Guid? customerId, OrderStatus? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsQueryable();

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }

    public Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(entity);
        return Task.CompletedTask;
    }
}
