using Vogel.BuildingBlocks.Infrastructure.Modularity;

namespace Vogel.Messanger.Infrastructure
{
    public class MessangerModuleBootstrapper : IModuleBootstrapper
    {
        public Task Bootstrap(IServiceProvider serviceProvider)
        {
            return Task.CompletedTask;
        }
    }
}
