using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.EntityFramework.Repositories;
using Vogel.MediaEngine.Domain;

namespace Vogel.MediaEngine.Infrastructure.EntityFramework.Repositories
{
    public class MediaEngineRepository<TEntity> : EFCoreRepository<TEntity, MediaEngineDbContext>, IMediaEngineRepository<TEntity>
        where TEntity : class, IEntity
    {
        public MediaEngineRepository(MediaEngineDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
