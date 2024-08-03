using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Globalization;
using Vogel.BuildingBlocks.MongoDb.Extensions;

namespace Vogel.BuildingBlocks.MongoDb
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity>
        where TEntity : IMongoEntity
    {
        protected FilterDefinitionBuilder<TEntity> Filter = Builders<TEntity>.Filter;
        protected IMongoCollection<TEntity> MongoDbCollection { get; }

        protected readonly IMongoDatabase MongoDatabase;

        public MongoRepository(IMongoDatabase mongoDatabase)
        {
            MongoDatabase = mongoDatabase;

            MongoDbCollection = MongoDatabase.GetCollection<TEntity>(MongoCollectionHelper.ResolveCollectionName(typeof(TEntity)));
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
            await MongoDbCollection.ReplaceOneAsync(Filter.Eq(x=> x.Id, entity.Id), entity );

            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            await MongoDbCollection.DeleteOneAsync(Filter.Eq(x => x.Id, id));
        }
        public async Task DeleteAsync(TEntity entity)
        {
            await DeleteAsync(entity.Id);
        }


        public IMongoQueryable<TEntity> AsQuerable()
        {
            return MongoDbCollection.AsQueryable();
        }
        public IMongoCollection<TEntity> AsMongoCollection()
        {
            return MongoDbCollection;
        }

        public async Task<TEntity?> FindByIdAsync(string id)
        {
            var filter = Filter.Eq(x => x.Id, id);
            var cursor = await MongoDbCollection.FindAsync(Filter.Eq(x=> x.Id, id));

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

        public IMongoQueryable<TEntity> AsQueryable()
        {
            return MongoDbCollection.AsQueryable();
        }

    }
}
