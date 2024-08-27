using AutoMapper;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Users;

namespace Vogel.Social.Application.Users.Profiles
{
    internal class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserMongoEntity>();
        }
    }
}
