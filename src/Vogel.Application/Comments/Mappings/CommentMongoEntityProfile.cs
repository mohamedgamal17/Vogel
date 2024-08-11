using AutoMapper;
using Vogel.Application.Extensions;
using Vogel.Domain.Comments;
using Vogel.MongoDb.Entities.Comments;

namespace Vogel.Application.Comments.Mappings
{
    public class CommentMongoEntityProfile : Profile
    {
        public CommentMongoEntityProfile()
        {
            CreateMap<Comment, CommentMongoEntity>()
                .MapAuditingProperties()
                .ReverseMap();
        }
    }
}
