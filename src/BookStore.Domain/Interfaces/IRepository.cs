using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedAsync(
        string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        string? sortBy, bool ascending, int page, int pageSize,
        CancellationToken cancellationToken = default);
    Task<bool> IsbnExistsAsync(string isbn, Guid? excludeBookId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetActiveAsync(CancellationToken cancellationToken = default);
}

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeCustomerId = null, CancellationToken cancellationToken = default);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetPagedAsync(
        Guid? customerId, string? status, int page, int pageSize,
        CancellationToken cancellationToken = default);
}

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
}
