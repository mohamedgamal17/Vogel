using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance
{
    public class MongoDbRepository<TEntity> : IMongoDbRepository<TEntity> where TEntity : Entity
    {
        private readonly IMongoDatabase _mongoDatabase;

        private readonly IMongoCollection<TEntity> _mongoCollection;

        private readonly MongoDbContext _mongoDbContext;
        public MongoDbRepository(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _mongoDatabase = _mongoDbContext.GetMongoDatabase();
            _mongoCollection = _mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name);

        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _mongoCollection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<List<TEntity>> InsertManyAsync(List<TEntity> entities)
        {
            await _mongoCollection.InsertManyAsync(entities);

            return entities;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await _mongoCollection.ReplaceOneAsync((x) => x.Id == entity.Id, entity);

            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await _mongoCollection.DeleteOneAsync((x) => x.Id == entity.Id);
        }


        public IQueryable<TEntity> AsQuerable()
        {
            return _mongoCollection.AsQueryable();
        }
        public IMongoCollection<TEntity> AsMongoCollection()
        {
            return _mongoCollection;
        }

        public async Task<TEntity?> FindByIdAsync(string id)
        {
            var filter = new FilterDefinitionBuilder<TEntity>()
                .Eq(x => x.Id, id);

            var cursor = await _mongoCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> FindAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await _mongoCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<TEntity> SingleAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await _mongoCollection.FindAsync(filter);

            return await cursor.FirstAsync();
        }

        public async Task<List<TEntity>> ApplyFilterAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await _mongoCollection.FindAsync(filter);

            return await cursor.ToListAsync();
        }
    }

}
