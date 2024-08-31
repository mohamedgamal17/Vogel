using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Social.Infrastructure;
namespace Vogel.Social.Application.Tests
{
    public class SocialApplicationTestModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.InstallModule<SocialModuleInstaller>(configuration, environment)
                .InstallModule<SocialApplicationTestModuleInstaller>(configuration, environment);
        }
    }
}
