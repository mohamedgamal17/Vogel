using MongoDB.Driver;
using System.Globalization;

namespace Vogel.BuildingBlocks.MongoDb
{
    public abstract class MongoRepository<TEntity,TKey> : MongoBase<TEntity>
        where TEntity : IMongoEntity<TKey> 
    {
        protected IMongoCollection<TEntity> MongoDbCollection { get; }

        protected readonly IMongoDatabase MongoDatabase;

        protected virtual string CollectionName => string.Format(CultureInfo.InvariantCulture, "{0}", typeof(TEntity).Name);

        protected MongoRepository(IMongoDatabase mongoDatabase)
        {
            MongoDatabase = mongoDatabase;

            MongoDbCollection = MongoDatabase.GetCollection<TEntity>(CollectionName);
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

        public async Task DeleteAsync(TEntity entity)
        {
            await MongoDbCollection.DeleteOneAsync(Filter.Eq(x=> x.Id , entity.Id));
        }


        public IQueryable<TEntity> AsQuerable()
        {
            return MongoDbCollection.AsQueryable();
        }
        public IMongoCollection<TEntity> AsMongoCollection()
        {
            return MongoDbCollection;
        }

        public async Task<TEntity?> FindByIdAsync(TKey id)
        {
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

    }
}
