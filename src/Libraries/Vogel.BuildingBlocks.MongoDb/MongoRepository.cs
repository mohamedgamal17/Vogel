using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Globalization;
using System.Linq.Expressions;
using Vogel.BuildingBlocks.MongoDb.Extensions;

namespace Vogel.BuildingBlocks.MongoDb
{
    public class MongoRepository<TEntity> : IMongoRepository<TEntity>
        where TEntity : IMongoEntity
    {
        public static FilterDefinitionBuilder<TEntity> Filter = Builders<TEntity>.Filter;

        public static UpdateDefinitionBuilder<TEntity> Update = Builders<TEntity>.Update;

        public static SortDefinitionBuilder<TEntity> Sort = Builders<TEntity>.Sort;

        public static ProjectionDefinitionBuilder<TEntity> Project = Builders<TEntity>.Projection;

        protected static readonly ReplaceOptions UpsertReplace =
                 new ReplaceOptions { IsUpsert = true };

        protected static readonly UpdateOptions Upsert = new UpdateOptions
        {
            IsUpsert = true
        };

        protected IMongoCollection<TEntity> MongoDbCollection { get; }

        protected readonly IMongoDatabase MongoDatabase;

        public MongoRepository(IMongoDatabase mongoDatabase)
        {
            MongoDatabase = mongoDatabase;

            MongoDbCollection = MongoDatabase.GetCollection<TEntity>(MongoCollectionHelper.ResolveCollectionName(typeof(TEntity)));
        }


        public async Task<TEntity> ReplaceOrInsertAsync(TEntity entity)
        {
            await MongoDbCollection.ReplaceOneAsync(Filter.Eq(x => x.Id, entity.Id), entity, UpsertReplace);

            return entity;
        }

        public async Task<IEnumerable<TEntity>> ReplaceOrInsertManyAsync(IEnumerable<TEntity> entities)
        {
            var writes = entities.Select(x =>
                    new ReplaceOneModel<TEntity>(Filter.Eq(y => y.Id, x.Id), x)
                    {
                        IsUpsert = true
                    }
                );

            await MongoDbCollection.BulkWriteAsync(writes);

            return entities;
        }

        public async Task<UpdateResult> UpdateAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            return await MongoDbCollection.UpdateOneAsync(filter, update, Upsert);
        }

        public async Task<UpdateResult> UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update)
        {
            return await MongoDbCollection.UpdateManyAsync(filter, update, Upsert);
        }

        public async Task<DeleteResult> DeleteAsync(string id)
        {
            return await MongoDbCollection.DeleteOneAsync(Filter.Eq(x => x.Id, id));
        }

        public async Task<DeleteResult> DeleteAsync(FilterDefinition<TEntity> filter)
        {
            return await MongoDbCollection.DeleteOneAsync(filter);
        }
        public async Task<DeleteResult> DeleteAsync(TEntity entity)
        {
            return await DeleteAsync(entity.Id);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<TEntity> filter)
        {
            return await MongoDbCollection.DeleteManyAsync(filter);
        }
        public IMongoQueryable<TEntity> AsQuerable()
        {
            return MongoDbCollection.AsQueryable();
        }
      
     
        public async Task<List<TEntity>> QueryAsync(FilterDefinition<TEntity> filter)
        {
            return await MongoDbCollection.Find(filter).ToListAsync();
        }

        public async IAsyncEnumerable<TEntity> StreamAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            while (await cursor.MoveNextAsync())
            {
                foreach (var entity in cursor.Current)
                {
                    yield return entity;
                }
            }
        }

        public async Task<TEntity?> FindByIdAsync(string id)
        {
            var filter = Filter.Eq(x => x.Id, id);
            var cursor = await MongoDbCollection.FindAsync(Filter.Eq(x => x.Id, id));

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

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.FirstAsync();
        }

        public async Task<TEntity?> SingleOrDefaultAsync(FilterDefinition<TEntity> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter)
        {
            var cursor = await MongoDbCollection.FindAsync(filter);

            return await cursor.FirstOrDefaultAsync();
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await MongoDbCollection.AsQueryable()
                .AnyAsync(expression);
        }

        public IMongoCollection<TEntity> AsMongoCollection()
        {
            return MongoDbCollection;
        }

        public IMongoQueryable<TEntity> AsQueryable()
        {
            return MongoDbCollection.AsQueryable();
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return MongoDatabase.GetCollection<T>(name);
        }

       
    }
}
