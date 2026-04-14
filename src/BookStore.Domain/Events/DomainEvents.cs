using BookStore.Domain.Common;
using BookStore.Domain.ValueObjets;

namespace BookStore.Domain.Events;

public sealed record BookCreatedEvent(Guid BookId, string Title) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt {  get; } = DateTime.UtcNow;
}

public sealed record BookPriceChangedEvent(Guid BookId, Money OldPrice, Money NewPrice) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt {  get; } = DateTime.UtcNow;
}

public sealed record BookOutOfStockEvent(Guid BookId, string Title) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt {  get; } = DateTime.UtcNow;
}

public sealed record OrderCreatedEvent(Guid OrderId, Guid CustomerId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt {  get; } = DateTime.UtcNow;
}

public sealed record OrderPaymentConfirmedEvent(Guid OrderId, Guid CustomerId, Money Total) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt {  get; } = DateTime.UtcNow;
}

public sealed record OrderShippedEvent(Guid OrderId, Guid CustomerId, Address ShippingAdress) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt {  get; } = DateTime.UtcNow;
}

public sealed record OrderCancelledEvent(Guid OrderId, Guid CustomerId, string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}


