using BookStore.Domain.Entities;
using BookStore.Domain.ValueObjects;

namespace BookStore.Domain.Interfaces;

public interface IBookRepository :IRepository<Book>
{
    Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedAsync (
        string? searchTerm, Guid? categoryId, decimal? minPrince, decimal? maxPrince, bool? sortyBy, 
        bool ascending, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<bool> IsbnExistsAsync(Isbn isbn, Guid? excludeBookId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Book>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
