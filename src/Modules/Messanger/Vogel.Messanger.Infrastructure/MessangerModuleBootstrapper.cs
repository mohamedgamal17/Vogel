using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Messanger.Infrastructure.EntityFramework;
namespace Vogel.Messanger.Infrastructure
{
    public class MessangerModuleBootstrapper : IModuleBootstrapper
    {
        public async Task Bootstrap(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<MessangerDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
