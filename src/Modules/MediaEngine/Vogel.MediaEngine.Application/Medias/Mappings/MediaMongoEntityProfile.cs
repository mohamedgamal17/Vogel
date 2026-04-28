using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Vogel.MediaEngine.Shared.Enums;

namespace Vogel.MediaEngine.Application.Medias.Mappings
{
    public class MediaMongoEntityProfile : Profile
    {
        public MediaMongoEntityProfile()
        {
            CreateMap<MediaType, MongoEntities.Medias.MediaType>()
                .ConvertUsingEnumMapping()
                .ReverseMap();

            CreateMap<Domain.Medias.Media, MongoEntities.Medias.MediaMongoEntity>()
                .ForMember(x => x.MediaType, opt => opt.MapFrom(c => c.MediaType));
        }
    }
}
