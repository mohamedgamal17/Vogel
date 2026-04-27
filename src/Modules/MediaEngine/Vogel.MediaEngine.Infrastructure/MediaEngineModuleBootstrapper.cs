using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.MediaEngine.Infrastructure.EntityFramework;

namespace Vogel.MediaEngine.Infrastructure
{
    public class MediaEngineModuleBootstrapper : IModuleBootstrapper
    {
        public async Task Bootstrap(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<MediaEngineDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
