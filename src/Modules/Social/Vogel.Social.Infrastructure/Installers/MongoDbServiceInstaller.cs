using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.MongoDb.Extensions;

namespace Vogel.Social.Infrastructure.Installers
{
    public class MongoDbServiceInstaller : IServiceInstaller
    {
        public void InstallAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.RegisterRepositoriesFromAssembly(MongoEntities.AssemblyReference.Assembly)
                .RegisterMigrationFromAssembly(MongoEntities.AssemblyReference.Assembly);
        }
    }
}
