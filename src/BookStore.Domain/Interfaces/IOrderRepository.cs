using BookStore.Domain.Entities;
using BookStore.Domain.Enums;

namespace BookStore.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Order> Orders, int totalCount)> GetPagedAsync(Guid? customerId, OrderStatus? status, int page, int pageSize, CancellationToken cancellationToken);
}
