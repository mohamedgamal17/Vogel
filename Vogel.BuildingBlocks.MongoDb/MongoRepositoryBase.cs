using MongoDB.Bson;
using MongoDB.Driver;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoRepository
    {
    }
    public abstract class MongoRepositoryBase<TEntity> : IMongoRepository
    {
        protected  readonly FilterDefinitionBuilder<TEntity> Filter =
            Builders<TEntity>.Filter;

        protected  readonly IndexKeysDefinitionBuilder<TEntity> Index =
            Builders<TEntity>.IndexKeys;

        protected  readonly ProjectionDefinitionBuilder<TEntity> Projection =
            Builders<TEntity>.Projection;

        protected  readonly SortDefinitionBuilder<TEntity> Sort =
            Builders<TEntity>.Sort;

        protected  readonly UpdateDefinitionBuilder<TEntity> Update =
            Builders<TEntity>.Update;

        protected  readonly BulkWriteOptions BulkUnordered =
            new BulkWriteOptions { IsOrdered = true };

        protected  readonly InsertManyOptions InsertUnordered =
            new InsertManyOptions { IsOrdered = true };

        protected readonly ReplaceOptions UpsertReplace =
            new ReplaceOptions { IsUpsert = true };

        protected readonly UpdateOptions Upsert =
            new UpdateOptions { IsUpsert = true };

        protected readonly BsonDocument FindAll =
            [];
    }
}
