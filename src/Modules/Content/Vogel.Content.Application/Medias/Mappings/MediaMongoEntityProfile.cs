using AutoMapper;
using AutoMapper.Extensions.EnumMapping;

namespace Vogel.Content.Application.Medias.Mappings
{
    public class MediaMongoEntityProfile : Profile
    {
        public MediaMongoEntityProfile()
        {
            CreateMap<Domain.Medias.MediaType, MongoEntities.Medias.MediaType>()
                .ConvertUsingEnumMapping()
                .ReverseMap();

            CreateMap<Domain.Medias.Media, MongoEntities.Medias.MediaMongoEntity>()
                .ForMember(x => x.MediaType, opt => opt.MapFrom(c => c.MediaType));
        }
    }
}
