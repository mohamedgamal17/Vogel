using Microsoft.Extensions.DependencyInjection;
using Vogel.MongoDb.Entities.Comments;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.Posts;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities
{
    public static class ServiceCollectionExtensions
    {   
        public static  IServiceCollection AddMongoDbEntites(this IServiceCollection services)
        {
            services.AddTransient<UserMongoRepository>();
            services.AddTransient<PostMongoRepository>();
            services.AddTransient<MediaMongoRepository>();
            services.AddTransient<PostReactionMongoRepository>();
            services.AddTransient<CommentMongoRepository>();
            return services;
        }
    }
}
