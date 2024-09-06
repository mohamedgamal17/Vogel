using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.MongoDb.Extensions;

namespace Vogel.Content.Infrastructure.Installers
{
    public class MongoDbServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.RegisterMongoRepositoriesFromAssembly(MongoEntities.AssemblyReference.Assembly)
                .RegisterMigrationFromAssembly(MongoEntities.AssemblyReference.Assembly);

        }
    }
}
