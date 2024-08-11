using AutoMapper;
using Vogel.Application.Extensions;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Friendship;

namespace Vogel.Application.Friendship.Mappings
{
    public class FriendshipProfile : Profile
    {
        public FriendshipProfile()
        {
            CreateMap<FriendRequest, FriendRequestMongoEntity>()
                .MapAuditingProperties();

            CreateMap<Friend, FriendMongoEntity>()
                .MapAuditingProperties();
        }
    }
}
