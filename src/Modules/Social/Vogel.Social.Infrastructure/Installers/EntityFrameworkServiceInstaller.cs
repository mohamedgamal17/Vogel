using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.Application;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Social.Domain;
using Vogel.Social.Infrastructure.EntityFramework;
using Vogel.Social.Infrastructure.EntityFramework.Repositories;

namespace Vogel.Social.Infrastructure.Installers
{
    public class EntityFrameworkServiceInstaller : IServiceInstaller
    {
        public void InstallAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContext<SocialDbContext>((sp, opt) =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"), cfg => cfg
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                   )
                .AddInterceptors(
                        sp.GetRequiredService<AuditableEntityInterceptors>(),
                        sp.GetRequiredService<DispatchDomainEventInterceptor>()
                   );
            });

            services.AddTransient(typeof(ISocialRepository<>), typeof(SocialRepository<>));
        }
    }
}
