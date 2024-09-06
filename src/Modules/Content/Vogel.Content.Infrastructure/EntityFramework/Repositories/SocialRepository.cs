using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.EntityFramework.Repositories;
using Vogel.Content.Domain;
namespace Vogel.Content.Infrastructure.EntityFramework.Repositories
{
    public class SocialRepository<TEntity> : EFCoreRepository<TEntity, ContentDbContext>, IContentRepository<TEntity>
        where TEntity : class, IEntity
    {
        public SocialRepository(ContentDbContext dbContext) : base(dbContext)
        {

        }
    }
}
