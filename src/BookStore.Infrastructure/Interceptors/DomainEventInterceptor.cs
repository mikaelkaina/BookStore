using BookStore.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookStore.Infrastructure.Interceptors;

public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;

    private readonly Dictionary<DbContext, List<IDomainEvent>> _events = new();

    public DomainEventInterceptor(IPublisher publisher)
        => _publisher = publisher;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var context = eventData.Context;

        var domainEvents = context.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        if (domainEvents.Count > 0)
        {
            _events[context] = domainEvents;

            context.ChangeTracker
                .Entries<Entity>()
                .ToList()
                .ForEach(e => e.Entity.ClearDomainEvents());
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var context = eventData.Context;

        if (!_events.TryGetValue(context, out var events) || events.Count == 0)
            return await base.SavedChangesAsync(eventData, result, cancellationToken);

        _events.Remove(context);

        foreach (var domainEvent in events)
            await _publisher.Publish(domainEvent, cancellationToken);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}