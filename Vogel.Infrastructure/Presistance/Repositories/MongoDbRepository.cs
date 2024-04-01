using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance.Repositories
{
    public class MongoDbRepository<TEntity> : IMongoDbRepository<TEntity> where TEntity : Entity
    {

        private readonly MongoDbContext _mongoDbContext;

        protected IMongoDatabase MongoDatabase { get; }
        protected IMongoCollection<TEntity> MongoDbCollection { get; }
        public MongoDbRepository(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
            MongoDatabase = _mongoDbContext.GetMongoDatabase();
            MongoDbCollection = MongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name);

        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await MongoDbCollection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<List<TEntity>> InsertManyAsync(List<TEntity> entities)
        {
            await MongoDbCollection.InsertManyAsync(entities);

            return entities;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await MongoDbCollection.ReplaceOneAsync((x) => x.Id == entity.Id, entity);

            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await MongoDbCollection.DeleteOneAsync((x) => x.Id == entity.Id);
        }


        public IQueryable<TEntity> AsQuerable()
        {
            return MongoDbCollection.AsQueryable();
        }
        public IMongoCollection<TEntity> AsMongoCollection()
        {
            return MongoDbCollection;
        }

        public async Task<TEntity?> FindByIdAsync(string id)
        {
            var filter = new FilterDefinitionBuilder<TEntity>()
                .Eq(x => x.Id, id);

            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> FindAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<TEntity> SingleAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.FirstAsync();
        }

        public async Task<List<TEntity>> ApplyFilterAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.ToListAsync();
        }
    }

}
