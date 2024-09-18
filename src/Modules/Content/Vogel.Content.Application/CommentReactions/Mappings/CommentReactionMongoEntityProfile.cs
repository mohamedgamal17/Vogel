using AutoMapper;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.CommentReactions;
namespace Vogel.Application.CommentReactions.Mappings
{
    public class CommentReactionMongoEntityProfile : Profile
    {
        public CommentReactionMongoEntityProfile()
        {
            CreateMap<CommentReaction, CommentReactionMongoEntity>();
               
        }
    }
}
