using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<Cart?> GetBySessionIdNoTrackingAsync(string sessionId, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
