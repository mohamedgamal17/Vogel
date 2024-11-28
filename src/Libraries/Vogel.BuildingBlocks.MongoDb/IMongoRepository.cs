using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoRepository
    {

    }
    public interface IMongoRepository<TEntity> : IMongoRepository
        where TEntity : IMongoEntity
    {
        Task<TEntity> ReplaceOrInsertAsync(TEntity entity);
        Task<IEnumerable<TEntity>> ReplaceOrInsertManyAsync(IEnumerable<TEntity> entities);
        Task<UpdateResult> UpdateAsync(string id, UpdateDefinition<TEntity> update);
        Task<UpdateResult> UpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update );
        Task<UpdateResult> UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update );
        Task<DeleteResult> DeleteAsync(string id);
        Task<DeleteResult> DeleteAsync(FilterDefinition<TEntity> filter);
        Task<DeleteResult> DeleteAsync(TEntity entity);
        Task<DeleteResult> DeleteManyAsync(FilterDefinition<TEntity> filter);
        IMongoQueryable<TEntity> AsQuerable();
        IMongoCollection<TEntity> AsMongoCollection();
        Task<TEntity?> FindByIdAsync(string id);
        Task<TEntity?> FindAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> SingleAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> SingleOrDefaultAsync(FilterDefinition<TEntity> filter);
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression);
        Task<List<TEntity>> QueryAsync(FilterDefinition<TEntity> filter);
        IAsyncEnumerable<TEntity> StreamAsync(FilterDefinition<TEntity> filter);
    }

}
