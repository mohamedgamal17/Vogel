using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Configuration;

namespace Vogel.BuildingBlocks.MongoDb.Migrations
{
    public class MongoMigrationEngine : IMongoMigrationEngine
    {
        const string MIGRATION_COLLECTION = "_migrations";

        private readonly IServiceProvider _serviceProvider;
        private readonly MongoDbSettings _mongoDbSettings;
        private readonly IMongoClient _mongoClient;

        public MongoMigrationEngine(IServiceProvider serviceProvider, MongoDbSettings mongoDbSettings, IMongoClient mongoClient)
        {
            _serviceProvider = serviceProvider;
            _mongoDbSettings = mongoDbSettings;
            _mongoClient = mongoClient;
        }

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();

            var db = _mongoClient.GetDatabase(_mongoDbSettings.Database);

            var migrationCollection = db.GetCollection<Migration>(MIGRATION_COLLECTION);

            var migrations = scope.ServiceProvider.GetServices<IMongoDbMigration>();

            int lastVersion = await GetLastMigrationVersion();

            foreach (var migration in migrations.Where(x=> x.Version > lastVersion))
            {
                await migration.Up(db);

                await migrationCollection.InsertOneAsync(new Migration
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = migration.Version
                });
            }
        }

        private async Task<int> GetLastMigrationVersion()
        {
            var db = _mongoClient.GetDatabase(_mongoDbSettings.Database);

            var collection = db.GetCollection<Migration>(MIGRATION_COLLECTION);

            var migration = collection.AsQueryable()
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();


            if (migration == null)
            {
                return 0;
            }

            return migration.Version;
        }

    }


    public interface IMongoMigrationEngine
    {
        Task MigrateAsync(CancellationToken cancellationToken = default);
    }
}
