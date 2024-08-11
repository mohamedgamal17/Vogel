using FluentAssertions;
using Vogel.Application.PostReactions.Commands;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Domain.Posts;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class PostReactionAssertionExtensions
    {

        public static void AssertPostReaction(this PostReaction postReaction , PostReactionCommandBase command) 
        {
            postReaction.PostId.Should().Be(command.PostId);
            postReaction.Type.Should().Be(command.Type);
        }

        public static void AssertPostReactionMongoEntity(this PostReactionMongoEntity mongoEntity , PostReaction postReaction)
        {
            mongoEntity.Id.Should().Be(postReaction.Id);
            mongoEntity.UserId.Should().Be(postReaction.UserId);
            mongoEntity.PostId.Should().Be(postReaction.PostId);
            mongoEntity.Type.Should().Be((MongoDb.Entities.PostReactions.ReactionType)postReaction.Type);
            mongoEntity.AssertAuditingProperties(postReaction);
        }
        
        public static void AssertPostReactionDto(this PostReactionDto dto , PostReaction postReaction , User user = null)
        {
            dto.Id.Should().Be(postReaction.Id);
            dto.PostId.Should().Be(postReaction.PostId);
            dto.UserId.Should().Be(postReaction.UserId);
            dto.Type.Should().Be((MongoDb.Entities.PostReactions.ReactionType)postReaction.Type);
            if(user != null)
            {
                dto.User.AssertUserDto(user);
            }
        }
    }
}
