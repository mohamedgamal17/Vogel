using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading;
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

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

            return  base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await DispatchDomainEvents(eventData.Context);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
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
                if(entry.Entity.Events.Count > 0)
                {
                    foreach (var @event in entry.Entity.Events)
                    {
                        await _mediator.Publish(@event);

                        entry.Entity.ClearDomainEvents();

                    }
                }           
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
