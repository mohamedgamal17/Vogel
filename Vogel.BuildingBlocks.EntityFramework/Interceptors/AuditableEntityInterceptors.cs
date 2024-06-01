using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.BuildingBlocks.EntityFramework.Interceptors
{
    public class AuditableEntityInterceptors : SaveChangesInterceptor
    {
        private readonly ISecurityContext _securityContext;
        private readonly TimeProvider _timeProvider;

        public AuditableEntityInterceptors(ISecurityContext securityContext, TimeProvider timeProvider)
        {
            _securityContext = securityContext;
            _timeProvider = timeProvider;
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            UpdateEntites(eventData.Context);

            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            UpdateEntites(eventData.Context);

            return base.SavedChangesAsync(eventData, result);
        }


        private void UpdateEntites(DbContext? dbContext)
        {
            if(dbContext == null)
            {
                return;
            }

            var entries = dbContext.ChangeTracker.Entries<IAuditedEntity>();

            foreach (var entry in entries)
            {
                var utcNow = _timeProvider.GetUtcNow();

                if (entry.State is EntityState.Added or EntityState.Modified )
                {      
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatorId = _securityContext.User?.Id;
                        entry.Entity.CreationTime = utcNow;
                    }
                    entry.Entity.ModifierId = _securityContext.User?.Id;
                    entry.Entity.ModificationTime = utcNow;
                }
                else
                {

                    entry.Entity.DeletorId = _securityContext.User?.Id;
                    entry.Entity.DeletionTime = utcNow;   
                }
            }
        }

    }
}
