using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.BuildingBlocks.Infrastructure.Security;
namespace Vogel.BuildingBlocks.EntityFramework.Interceptors
{
    public class AuditableEntityInterceptors : SaveChangesInterceptor
    {
        private readonly ISecurityContext _securityContext;

        public AuditableEntityInterceptors(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }


        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntites(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
             UpdateEntites(eventData.Context);

            return base.SavingChangesAsync(eventData, result);
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
                if (entry.State is EntityState.Added or EntityState.Modified )
                {      
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.CreatorId = _securityContext.User?.Id;
                        entry.Entity.CreationTime = DateTime.UtcNow;
                    }
                    entry.Entity.ModifierId = _securityContext.User?.Id;
                    entry.Entity.ModificationTime = DateTime.UtcNow;
                }
                else if(entry.State == EntityState.Deleted)
                {

                    entry.Entity.DeletorId = _securityContext.User?.Id;
                    entry.Entity.DeletionTime = DateTime.UtcNow;   
                }
            }
        }

    }
}
