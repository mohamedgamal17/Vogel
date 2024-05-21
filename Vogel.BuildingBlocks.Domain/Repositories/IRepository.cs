using System.Linq.Expressions;

namespace Vogel.BuildingBlocks.Domain.Repositories
{
    public interface IRepository <TEntity> where TEntity : class
    {
        IQueryable<TEntity> AsQuerable();
        Task DeleteAsync(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<List<TEntity>> InsertManyAsync(List<TEntity> entities);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity?> FindByIdAsync(object id);
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression);
    }
}
