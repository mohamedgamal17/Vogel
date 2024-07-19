using AutoMapper;
using Vogel.Application.Extensions;
using Vogel.Domain.Comments;
using Vogel.MongoDb.Entities.CommentReactions;

namespace Vogel.Application.CommentReactions.Mappings
{
    public class CommentReactionMongoEntityProfile : Profile
    {
        public CommentReactionMongoEntityProfile()
        {
            CreateMap<CommentReaction, CommentReactionMongoEntity>()
                .MapAuditingProperties();
        }
    }
}
