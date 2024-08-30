using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.MongoDb.Migrations;

namespace Vogel.Host
{
    public class HostModuleBootstrapper : IModuleBootstrapper
    {
        public async Task Bootstrap(IServiceProvider serviceProvider)
        {
           await MigrateMongoDatabase(serviceProvider);
        }

        private async Task MigrateMongoDatabase(IServiceProvider serviceProvider)
        {
            var mongoEngine = serviceProvider.GetRequiredService<IMongoMigrationEngine>();

            await mongoEngine.MigrateAsync();
        }
    }
}
