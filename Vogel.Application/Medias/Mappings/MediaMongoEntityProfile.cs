using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Vogel.Application.Extensions;
namespace Vogel.Application.Medias.Mappings
{
    public class MediaMongoEntityProfile :  Profile
    {
        public MediaMongoEntityProfile()
        {
            CreateMap<Domain.Medias.MediaType, MongoDb.Entities.Medias.MediaType>()     
                .ConvertUsingEnumMapping()
                .ReverseMap();

            CreateMap<Domain.Medias.Media, MongoDb.Entities.Medias.MediaMongoEntity>()
                .ForMember(x=> x.MediaType, opt=> opt.MapFrom(c=> c.MediaType))
                .MapAuditingProperties();
        }
    }
}
