namespace BookStore.Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
