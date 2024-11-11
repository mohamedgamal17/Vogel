using FastEndpoints.Swagger;
using MassTransit;
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

            ConfigureMassTransit(services, configuration);

            ConfigureSwagger(services);

            ConfigureSignalRHubs(services);

            RegisterControllers(services);

            services.RegisterMediatRCommonPibelineBehaviors()
                .RegisterEfCoreInterceptors()
                .AddHttpContextAccessor()
                .InstallModulesFromAssembly(
                    configuration,
                    environment,
                     BuildingBlocks.Infrastructure.AssemblyReference.Assembly,
                     Social.Infrastructure.AssemblyReference.Assembly,
                     Content.Infrastructure.AssemblyReference.Assembly,
                     Messanger.Infrastructure.AssemblyReference.Assembly
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

        private void ConfigureMassTransit(IServiceCollection services , IConfiguration configuration)
        {
            services.AddMassTransit(busRegisterConfig =>
            {
                busRegisterConfig.UsingRabbitMq((ctx, rabbitMqConfig) =>
                {
                    string rabbitMqHost = configuration.GetValue<string>("RabbitMq:Host")!;
                    string userName = configuration.GetValue<string>("RabbitMq:UserName")!;
                    string password = configuration.GetValue<string>("RabbitMq:Password")!;
                    rabbitMqConfig.Host(rabbitMqHost, hostConfig =>
                    {
                        hostConfig.Username(userName);
                        hostConfig.Password(password);
                    });

                    rabbitMqConfig.ConfigureEndpoints(ctx);
                });

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

        private void ConfigureSignalRHubs(IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}
