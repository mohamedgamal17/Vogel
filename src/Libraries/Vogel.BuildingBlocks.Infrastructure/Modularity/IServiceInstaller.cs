using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vogel.BuildingBlocks.Infrastructure.Modularity
{
    public interface IServiceInstaller
    {
        void InstallAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
    }
}
