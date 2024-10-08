using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.EntityFramework.Repositories;
using Vogel.Messanger.Domain;

namespace Vogel.Messanger.Infrastructure.EntityFramework.Repositories
{
    public class MessangerRepository<TEntity> : EFCoreRepository<TEntity, MessangerDbContext>, IMessangerRepository<TEntity>
        where TEntity : class, IEntity
    {
        public MessangerRepository(MessangerDbContext dbContext) : base(dbContext)
        {

        }
    }
}
