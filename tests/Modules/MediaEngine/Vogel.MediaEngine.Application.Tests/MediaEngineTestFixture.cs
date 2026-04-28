using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn.Graph;
using Vogel.Application.Tests;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.Infrastructure.EntityFramework;

namespace Vogel.MediaEngine.Application.Tests
{
    [TestFixture]
    public class MediaEngineTestFixture : TestFixture
    {
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<MediaEngineApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }

        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await services.RunModulesBootstrapperAsync();
            await ResetSqlDb(services);
            await DropMongoDb(services);
            await SeedData(services);
        }

        private static async Task SeedData(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<MediaEngineDbContext>();
            var medias = new List<Media>();
            for (var i = 0; i < 20; i++)
            {
                medias.Add(new Media
                {
                    File = Guid.NewGuid().ToString(),
                    MimeType = "image/png",
                    MediaType = MediaType.Image,
                    Size = 1024 + i,
                    UserId = $"user-{i % 5}",
                });
            }

            await dbContext.AddRangeAsync(medias);
            await dbContext.SaveChangesAsync();
        }

        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await ResetSqlDb(services);
            await DropMongoDb(services);
        }

        protected static async Task ResetSqlDb(IServiceProvider services)
        {
            var config = services.GetRequiredService<IConfiguration>();
            try
            {
                var respwan = await Respawn.Respawner.CreateAsync(config.GetConnectionString("Default")!, new Respawn.RespawnerOptions
                {
                    TablesToIgnore = new Table[]
                    {
                        "sysdiagrams",
                        "tblUser",
                        "tblObjectType",
                        "__EFMigrationsHistory",
                    },
                    SchemasToInclude = new string[]
                    {
                        "MediaEngine",
                    },
                });

                await respwan.ResetAsync(config.GetConnectionString("Default")!);
            }
            catch (InvalidOperationException)
            {
                // Database may not be initialized yet on first test run.
            }
        }
    }
}
