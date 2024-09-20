using AutoMapper;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
namespace Vogel.Social.Application.Friendship.Mappings
{
    public class FriendshipProfile : Profile
    {
        public FriendshipProfile()
        {
            CreateMap<FriendRequest, FriendRequestMongoEntity>();

            CreateMap<Friend, FriendMongoEntity>();

        }
    }
}
