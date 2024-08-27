using AutoMapper;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;

namespace Vogel.Social.Application.Pictures.Profiles
{
    internal class PictureProfile  : Profile
    {
        public PictureProfile()
        {
            CreateMap<Picture, PictureMongoEntity>();
        }
    }
}
