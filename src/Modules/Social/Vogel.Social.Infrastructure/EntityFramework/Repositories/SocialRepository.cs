using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.EntityFramework.Repositories;
using Vogel.Social.Domain;

namespace Vogel.Social.Infrastructure.EntityFramework.Repositories
{
    public class SocialRepository<TEntity> : EFCoreRepository<TEntity, SocialDbContext>, ISocialRepository<TEntity>
        where TEntity : class, IEntity
    {
        public SocialRepository(SocialDbContext dbContext) : base(dbContext)
        {

        }
    }
}
