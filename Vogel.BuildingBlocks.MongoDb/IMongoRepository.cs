using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoRepository
    {

    }
    public interface IMongoRepository<TEntity > : IMongoRepository
        where TEntity : IMongoEntity
    {
        Task<TEntity> InsertAsync(TEntity entity);
        Task<List<TEntity>> InsertManyAsync(List<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(string id);
        Task DeleteAsync(TEntity entity);
        IMongoQueryable<TEntity> AsQuerable();
        IMongoCollection<TEntity> AsMongoCollection();
        Task<TEntity?> FindByIdAsync(string id);
        Task<TEntity?> FindAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> SingleAsync(FilterDefinition<TEntity> filter);
        Task<List<TEntity>> ApplyFilterAsync(FilterDefinition<TEntity> filter);
    
    }
 
}
