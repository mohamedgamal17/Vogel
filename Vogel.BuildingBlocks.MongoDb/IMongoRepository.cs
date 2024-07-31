using MongoDB.Bson;
using MongoDB.Driver;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoRepository
    {

    }
    public interface IMongoRepository<TEntity , TKey> : IMongoRepository
        where TEntity : IMongoEntity<TKey> 
    {
        Task<TEntity> InsertAsync(TEntity entity);
        Task<List<TEntity>> InsertManyAsync(List<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey id);
        Task DeleteAsync(TEntity entity);
        IQueryable<TEntity> AsQuerable();
        IMongoCollection<TEntity> AsMongoCollection();
        Task<TEntity?> FindByIdAsync(TKey id);
        Task<TEntity?> FindAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> SingleAsync(FilterDefinition<TEntity> filter);
        Task<List<TEntity>> ApplyFilterAsync(FilterDefinition<TEntity> filter);
    
    }
 
}
