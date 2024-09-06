using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.EntityFramework.Repositories;
using Vogel.Content.Domain;
namespace Vogel.Content.Infrastructure.EntityFramework.Repositories
{
    public class ContentRepository<TEntity> : EFCoreRepository<TEntity, ContentDbContext>, IContentRepository<TEntity>
        where TEntity : class, IEntity
    {
        public ContentRepository(ContentDbContext dbContext) : base(dbContext)
        {

        }
    }
}
