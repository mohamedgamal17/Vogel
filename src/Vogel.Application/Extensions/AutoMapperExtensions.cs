using AutoMapper;
using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Application.Extensions
{
    public static class AutoMapperExtensions
    {

        public static IMappingExpression<TSource, TDestination> MapAuditingProperties<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping)
            where TSource : IAuditedEntity
            where TDestination : IMongoAuditing
        {
            return mapping.ForMember(x => x.CreationTime, opt => opt.MapFrom(c => c.CreationTime))
                .ForMember(x => x.CreatorId, opt => opt.MapFrom(c => c.CreatorId))
                .ForMember(x => x.ModificationTime, opt => opt.MapFrom(c => c.ModificationTime))
                .ForMember(x => x.ModifierId, opt => opt.MapFrom(c => c.ModifierId))
                .ForMember(x => x.DeletionTime, opt => opt.MapFrom(c => c.DeletionTime))
                .ForMember(x => x.DeletorId, opt => opt.MapFrom(c => c.DeletorId));
        }
    }
}
