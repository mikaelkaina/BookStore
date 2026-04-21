using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
        => _context = context;

    public async Task AddAsync(Category entity, CancellationToken cancellationToken = default)
        => await _context.Categories.AddAsync(entity, cancellationToken);
    public Task DeleteAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _context.Categories.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _context.Categories
        .AsNoTracking()
        .Where(c => c.IsActive)
        .OrderBy(c => c.Name)
        .ToListAsync(cancellationToken);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        => await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
        => await _context.Categories
        .AsNoTracking()
        .AnyAsync(c => 
        c.Slug == slug && 
        (excludeId == null || c.Id != excludeId.Value), cancellationToken);

    public Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(entity);
        return Task.CompletedTask;
    }
}
