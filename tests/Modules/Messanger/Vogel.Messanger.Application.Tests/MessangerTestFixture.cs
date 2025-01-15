using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn.Graph;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
namespace Vogel.Messanger.Application.Tests
{
    [TestFixture]
    public class MessangerTestFixture : TestFixture
    {
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<MessangerApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await ResetSqlDb(services);
            await DropMongoDb(services);
            await services.RunModulesBootstrapperAsync();
        }


        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await ResetSqlDb(services);
            await DropMongoDb(services);
        }

        protected async Task ResetSqlDb(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();

            var respwan = await Respawn.Respawner.CreateAsync(config.GetConnectionString("Default")!, new Respawn.RespawnerOptions
            {
                TablesToIgnore = new Table[]
                {
                  "sysdiagrams",
                  "tblUser",
                  "tblObjectType",
                  "__EFMigrationsHistory"
                },
                SchemasToInclude = new string[]
                {
                    "Messanger"
                }

            });

            await respwan.ResetAsync(config.GetConnectionString("Default")!);
        }
    }
}
