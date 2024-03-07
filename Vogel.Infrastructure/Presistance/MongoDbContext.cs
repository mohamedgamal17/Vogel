using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System.Reflection;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance
{
    public class MongoDbContext
    {
        public IMongoClient MongoClient { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public MongoDbSettings MongoDbSettings { get; private set; }

        public MongoDbContext(IMongoClient mongoClient, IServiceProvider serviceProvider, MongoDbSettings mongoDbSettings)
        {
            MongoClient = mongoClient;
            ServiceProvider = serviceProvider;
            MongoDbSettings = mongoDbSettings;
        }

        public void Initialize()
        {

            InitializeNamingConvention();

            InitializeClassMapping();
        }



        private void InitializeNamingConvention()
        {
            var pack = new ConventionPack();

            pack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register(
              "Camel Case Convention",
              pack,
              t => true);
        }

        private void InitializeClassMapping()
        {

            var assembly = Assembly.GetAssembly(typeof(IMongoDbClassMap<>))!;

            var classMaps = assembly
                .GetTypes()
                .Where(x => x.GetInterfaces()
                        .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMongoDbClassMap<>))
                    )
                .ToList();


            foreach (var classMap in classMaps)
            {
                var interfactType = classMap.GetInterfaces()
                     .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMongoDbClassMap<>));

                var genericType = interfactType.GetGenericArguments()[0];

                var obj = Activator.CreateInstance(classMap);

                var bsonClassMapType = typeof(BsonClassMap<>).MakeGenericType(genericType);

                var bsonClassMap = Activator.CreateInstance(bsonClassMapType)!;

                var methodInfo = classMap.GetMethod("Map")!;

                methodInfo.Invoke(obj, new object[] { bsonClassMap });

                BsonClassMap.RegisterClassMap((BsonClassMap)bsonClassMap);
            }
        }

        public IMongoDatabase GetMongoDatabase()
        {
            return MongoClient.GetDatabase(MongoDbSettings.Database);
        }
    }
}
