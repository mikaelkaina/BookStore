using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Persistence.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context)
        => _context = context;
    public async Task AddAsync(Book entity, CancellationToken cancellationToken = default)
        => await _context.Books.AddAsync(entity, cancellationToken);

    public Task DeleteAsync(Book entity, CancellationToken cancellationToken = default)
    {
        _context.Books.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => await _context.Books
        .Include(b => b.Category)
        .Where(b => b.CategoryId == categoryId && b.IsActive)
        .OrderBy(b => b.Title)
        .ToListAsync(cancellationToken);

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Books.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<Book?> GetByIsbnAsync(Isbn isbn, CancellationToken cancellationToken = default)
        => await _context.Books.FirstOrDefaultAsync(b => b.Isbn.Value == isbn.Value, cancellationToken);

    public async Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        bool? sortByPrice, bool ascending, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Books
            .Include(b => b.Category)
            .Where(b => b.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(b =>
            b.Title.Contains(searchTerm) ||
            b.Author.Contains(searchTerm) ||
            b.Publisher.Contains(searchTerm));

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where(b => b.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(b => b.Price.Amount <= maxPrice.Value);

        query = sortByPrice == true
            ? ascending
            ? query.OrderBy(b => b.Price.Amount)
                : query.OrderByDescending(b => b.Price.Amount)
                : ascending
            ? query.OrderBy(b => b.Title)
                : query.OrderByDescending(b => b.Title);

        var totalCount = await query.CountAsync(cancellationToken);

        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (books, totalCount);
    }

    public async Task<bool> IsbnExistsAsync(Isbn isbn, Guid? excludeBookId = null, CancellationToken cancellationToken = default)
        => await _context.Books
        .AnyAsync(b => 
        b.Isbn.Value == isbn.Value && 
        (excludeBookId == null || b.Id != excludeBookId.Value), cancellationToken);

    public Task UpdateAsync(Book entity, CancellationToken cancellationToken = default)
    {
        _context.Books.Update(entity);
        return Task.CompletedTask;
    }

    public async Task<Book?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    => await _context.Books
        .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
}
