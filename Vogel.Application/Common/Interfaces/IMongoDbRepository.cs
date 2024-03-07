using MongoDB.Driver;
using Vogel.Domain;

namespace Vogel.Application.Common.Interfaces
{
    public interface IMongoDbRepository<TEntity> where TEntity : Entity
    {
        IMongoCollection<TEntity> AsMongoCollection();
        IQueryable<TEntity> AsQuerable();
        Task DeleteAsync(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<List<TEntity>> InsertManyAsync(List<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity?> FindByIdAsync(string id);
        Task<TEntity?> FindAsync(FilterDefinition<TEntity> filter);
        Task<List<TEntity>> ApplyFilterAsync(FilterDefinition<TEntity> filter);
        Task<TEntity> SingleAsync(FilterDefinition<TEntity> filter);

    }
}