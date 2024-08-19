namespace Vogel.BuildingBlocks.Infrastructure.Modularity
{
    public interface IModuleBootstrapper 
    {
        Task Bootstrap(IServiceProvider serviceProvider);
    }
}
