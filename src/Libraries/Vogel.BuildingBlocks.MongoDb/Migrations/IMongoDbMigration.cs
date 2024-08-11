using MongoDB.Driver;

namespace Vogel.BuildingBlocks.MongoDb.Migrations
{
    public interface IMongoDbMigration
    {
        int Version { get; }
        Task Up(IMongoDatabase mongoDb);
    }
}
