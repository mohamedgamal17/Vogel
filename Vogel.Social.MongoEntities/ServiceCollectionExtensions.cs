using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Vogel.BuildingBlocks.MongoDb.Extensions;
namespace Vogel.Social.MongoEntities
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbEntites(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddVogelMongoDb(opt =>
            {
                opt.ConnectionString = configuration.GetValue<string>("MongoDb:ConnectionString")!;
                opt.Database = configuration.GetValue<string>("MongoDb:Database")!;
            })
            .AssemblyRegisterRepositories(Assembly.GetExecutingAssembly())
            .AssemblyRegisterMigration(Assembly.GetExecutingAssembly());



            return services;
        }
    }
}
