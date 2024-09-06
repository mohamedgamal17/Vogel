using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.BuildingBlocks.EntityFramework.Interceptors;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.Content.Domain;
using Vogel.Content.Infrastructure.EntityFramework;
using Vogel.Content.Infrastructure.EntityFramework.Repositories;
namespace Vogel.Content.Infrastructure.Installers
{
    public class EntityFrameworkServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddDbContext<ContentDbContext>((sp, opt) =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("Default"), cfg => cfg
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                   )
                .AddInterceptors(
                        sp.GetRequiredService<AuditableEntityInterceptors>(),
                        sp.GetRequiredService<DispatchDomainEventInterceptor>()
                   );
            });

            services.AddTransient(typeof(IContentRepository<>), typeof(ContentRepository<>));
        }
    }
}
