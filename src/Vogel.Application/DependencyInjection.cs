using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Vogel.Application.Comments.Polices;
using Vogel.Application.Medias.Policies;
using Vogel.Application.Posts.Policies;
using Vogel.MongoDb.Entities;
using Vogel.BuildingBlocks.Application;
namespace Vogel.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddMongoDbEntites(configuration);

            services.AddVogelCore(Assembly.GetExecutingAssembly());

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            ConfigureAuthroizationPolicies(services);

            return services;
        }
        private static void ConfigureAuthroizationPolicies(IServiceCollection services)
        {
            services.AddTransient<IAuthorizationHandler, PostOperationAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, IsMediaOwnerAuthorizationHandler>();
            services.AddTransient<IAuthorizationHandler, CommentOperationAuthorizationHandler>();
        }

    }
}
