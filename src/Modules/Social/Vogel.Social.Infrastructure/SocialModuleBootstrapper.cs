using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Social.Infrastructure.EntityFramework;

namespace Vogel.Social.Infrastructure
{
    public class SocialModuleBootstrapper : IModuleBootstrapper
    {
        public async Task Bootstrap(IServiceProvider serviceProvider)
        {
            var dbContext =  serviceProvider.GetRequiredService<SocialDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
