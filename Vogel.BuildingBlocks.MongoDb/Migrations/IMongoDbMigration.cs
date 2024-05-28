using MongoDB.Driver;

namespace Vogel.BuildingBlocks.MongoDb.Migrations
{
    public interface IMongoDbMigration
    {
        int Version { get; set; }
        Task Up(IMongoDatabase mongoDb);
    }
}
