using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.MediaEngine.Infrastructure;

namespace Vogel.MediaEngine.Application.Tests
{
    public class MediaEngineApplicationTestModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.InstallModule<MediaEngineModuleInstaller>(configuration, environment)
                .InstallModule<ApplicationTestModuleInstaller>(configuration, environment);

            services.AddVogelMongoDb(opt =>
            {
                opt.ConnectionString = configuration.GetValue<string>("MongoDb:ConnectionString")!;
                opt.Database = configuration.GetValue<string>("MongoDb:Database")!;
            });
        }
    }
}
