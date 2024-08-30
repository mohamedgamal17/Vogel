using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Vogel.BuildingBlocks.Application;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Modularity;
using Vogel.BuildingBlocks.MongoDb.Extensions;
namespace Vogel.Host
{
    public class HostModuleInstaller : IModuleInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            ConfigureAuthentication(services, configuration);

            ConfigureAuthorization(services);

            ConfigureMongoDb(services, configuration);

            ConfigureSwagger(services);

            RegisterControllers(services);

            services.RegisterMediatRCommonPibelineBehaviors()
                .RegisterEfCoreInterceptors()
                .AddHttpContextAccessor()
                .InstallModulesFromAssembly(
                    configuration,
                    environment,
                     BuildingBlocks.Infrastructure.AssemblyReference.Assembly,
                     Social.Infrastructure.AssemblyReference.Assembly
                );

            services.AddTransient<IModuleBootstrapper, HostModuleBootstrapper>();
        }

        private void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = configuration.GetValue<string>("IdentityProvider:Authority");
                options.Audience = configuration.GetValue<string>("IdentityProvider:Audience");
            });
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization();
        }

        private void ConfigureMongoDb(IServiceCollection services , IConfiguration configuration)
        {
            services.AddVogelMongoDb(opt =>
            {
                opt.ConnectionString = configuration.GetValue<string>("MongoDb:ConnectionString")!;
                opt.Database = configuration.GetValue<string>("MongoDb:Database")!;
            });
        }

        private void RegisterControllers(IServiceCollection services)
        {
            services.AddControllers();
        }
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.SwaggerDocument();
        }
    }
}
