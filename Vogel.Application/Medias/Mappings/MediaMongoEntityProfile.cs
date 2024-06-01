using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace Vogel.Application.Medias.Mappings
{
    public class MediaMongoEntityProfile :  Profile
    {
        public MediaMongoEntityProfile()
        {
            CreateMap<Domain.Medias.MediaType, MongoDb.Entities.Medias.MediaType>()
                .ConvertUsingEnumMapping()
                .ReverseMap();

            CreateMap<Domain.Medias.Media, MongoDb.Entities.Medias.MediaMongoEntity>();
        }
    }
}
