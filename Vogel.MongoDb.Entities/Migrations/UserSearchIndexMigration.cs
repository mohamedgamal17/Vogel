using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Migrations
{
    public class UserSearchIndexMigration : IMongoDbMigration
    {
        public int Version => 1722885800;

        public async Task Up(IMongoDatabase mongoDb)
        {
            var indexDefination = Builders<UserMongoEntity>.IndexKeys.Text(x => x.FirstName)
                .Text(x => x.LastName);

            var indexModel = new CreateIndexModel<UserMongoEntity>(indexDefination);

            await mongoDb.GetCollection<UserMongoEntity>("users").Indexes.CreateOneAsync(indexModel);
        }
    }
}
