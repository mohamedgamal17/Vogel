using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Content.Infrastructure.EntityFramework;
namespace Vogel.Content.Infrastructure
{
    public class ContentModuleBootstrapper : IModuleBootstrapper
    {
        public async Task Bootstrap(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ContentDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
