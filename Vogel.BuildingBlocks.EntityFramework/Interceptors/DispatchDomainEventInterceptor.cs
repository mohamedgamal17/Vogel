using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Events;
namespace Vogel.BuildingBlocks.EntityFramework.Interceptors
{ 
    public class DispatchDomainEventInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        public DispatchDomainEventInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(eventData.Context);

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

            return base.SavedChanges(eventData, result);
        }


        private async Task DispatchDomainEvents(DbContext? dbContext)
        {
            if(dbContext == null)
            {
                return;
            }

            var entries = dbContext.ChangeTracker.Entries();

            await PublishBasicCrudEvents(entries);

            var aggregateRootEntries = dbContext.ChangeTracker.Entries<IAggregateRoot>();

            foreach (var entry in aggregateRootEntries)
            {
                var events = entry.Entity.Events;

                await _mediator.Publish(events);

                entry.Entity.ClearDomainEvents();
            }
        }


        private async Task PublishBasicCrudEvents(IEnumerable<EntityEntry> entries)
        {
            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();

                if(entry.State == EntityState.Added)
                {
                    var eventType = typeof(EntityCreatedEvent<>).MakeGenericType(entityType);

                    var @event = Activator.CreateInstance(eventType, new object[] { entry.Entity})!;

                    await _mediator.Publish(@event);

                }else if(entry.State == EntityState.Modified)
                {
                    var eventType = typeof(EntityUpdatedEvent<>).MakeGenericType(entityType);

                    var @event = Activator.CreateInstance(eventType, new object[] { entry.Entity })!;

                    await _mediator.Publish(@event);
                }
                else
                {
                    var eventType = typeof(EntityDeletedEvent<>).MakeGenericType(entityType);

                    var @event = Activator.CreateInstance(eventType, new object[] { entry.Entity })!;

                    await _mediator.Publish(@event);
                }

            }
        }



    }
}
