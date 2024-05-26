using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Repositories;
namespace Vogel.BuildingBlocks.EntityFramework.Repositories
{
    public class EFCoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly DbContext _dbContext;

        public EFCoreRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> AsQuerable()
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TEntity?> FindByIdAsync(object id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<List<TEntity>> InsertManyAsync(List<TEntity> entities)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);

            await _dbContext.SaveChangesAsync();

            return entities;
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbContext.Set<TEntity>().SingleAsync(expression);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _dbContext.Set<TEntity>().SingleOrDefaultAsync(expression);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
             _dbContext.Set<TEntity>().Update(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
