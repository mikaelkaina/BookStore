namespace BookStore.Domain.Common;

public abstract class Entity
{
    private readonly List<IDomainEvent> domainEvents = [];

    public Guid Id { get; protected set; } =  Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdateAt { get; protected set; }

   public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

   protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => domainEvents.Add(domainEvent);

    public void ClearDomainEvents()
        => domainEvents.Clear();

    protected void SetUpdateAt()
        => UpdateAt = DateTime.UtcNow;
}
