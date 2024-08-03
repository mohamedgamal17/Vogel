using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;
using Vogel.BuildingBlocks.MongoDb.Exceptions;
using Vogel.BuildingBlocks.MongoDb.Migrations;
namespace Vogel.BuildingBlocks.MongoDb.Configuration
{
    public class VogelMongoDbBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public VogelMongoDbBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;

            var pack = new ConventionPack();
            pack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register(
              "Camel Case Convention",
              pack,
              t => true);

        }


        public VogelMongoDbBuilder AddRepository<T>() where T : IMongoRepository
        {
            return AddRepository(typeof(T));
        }

        public VogelMongoDbBuilder AddRepository(Type repositoryType)
        {
            if (!typeof(IMongoRepository).IsAssignableFrom(repositoryType))
            {
                throw new VogelMongoDbBuilderException($"Class : ({repositoryType.AssemblyQualifiedName}). Must implement " +
                    $"(${typeof(IMongoRepository).AssemblyQualifiedName}) to be able to register as mongo db repository");
            }


            _serviceCollection.AddTransient(repositoryType);

            return this;
        }

        public VogelMongoDbBuilder AssemblyRegisterRepositories(Assembly assembly)
        {
            var repositoriesTypes = assembly.GetTypes()
                .Where(x => x.IsClass && (x.BaseType?.IsGenericType ?? false))
                .Where(x => x.BaseType?.GetGenericTypeDefinition() == typeof(MongoRepository<>))
                .ToList();

            foreach (var type in repositoriesTypes)
            {
                AddRepository(type);
            }

           
            return this;
        }

        public VogelMongoDbBuilder AddMigration<TMigration>() where TMigration : class,IMongoDbMigration
        {
            return AddMigration(typeof(TMigration));
        }


        public VogelMongoDbBuilder AddMigration(Type type)
        {
            bool isAssignable = typeof(IMongoDbMigration).IsAssignableFrom(type);
            if (!isAssignable)
            {
                throw new VogelMongoDbBuilderException($"Class : ({type.AssemblyQualifiedName}). Must implement " +
                    $"(${typeof(IMongoDbMigration).AssemblyQualifiedName}) to be able to register as mongo db migration");
            }

            _serviceCollection.AddTransient(typeof(IMongoDbMigration), type);

            return this;
        }

        public VogelMongoDbBuilder AssemblyRegisterMigration(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(x => x.IsClass)
                .Where(x => x.GetInterfaces().Any(c => c == typeof(IMongoDbMigration)))
                .ToList();

            foreach (var type in types)
            {
                AddMigration(type);
            }

            return this;
        }
    }
}
