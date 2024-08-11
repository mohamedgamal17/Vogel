using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Vogel.Application.Extensions;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.Users.Mappings
{
    public class UserMongoEntityProfile : Profile
    {
        public UserMongoEntityProfile()
        {
            CreateMap<Domain.Users.Gender, MongoDb.Entities.Users.Gender>().ConvertUsingEnumMapping();

            CreateMap<User, UserMongoEntity>()
                .ForMember(x => x.Gender, opt => opt.MapFrom(c => c.Gender))
                .MapAuditingProperties();
                
        }
    }
}
