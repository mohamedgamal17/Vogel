using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Domain;
using Vogel.Content.MongoEntities.CommentReactions;
using Bogus;
using Vogel.Content.Application.CommentReactions.Commands.UpdateCommentReaction;
using Vogel.Content.Domain.Common;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.Tests.Extensions;
namespace Vogel.Content.Application.Tests.CommentReactions
{
    public class UpdateCommentReactionCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Comment> CommentRepository { get; }
        public IContentRepository<Post> PostRepository { get; }
        public IContentRepository<CommentReaction> CommentReactionRepository { get; }
        public IMongoRepository<CommentReactionMongoEntity> CommentReactionMongoRepository { get; }

        public UpdateCommentReactionCommandHandlerTests()
        {
            CommentRepository = ServiceProvider.GetRequiredService<IContentRepository<Comment>>();
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            CommentReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<CommentReaction>>();
            CommentReactionMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<CommentReactionMongoEntity>>();
        }

        [Test]
        public async Task Should_update_comment_reaction()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var fakePost = await CreateFakePost(userId);

            var fakeComment = await CreateFakeComment(fakePost.Id, userId);

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment.Id,userId);

            var command = new UpdateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                ReactionId = fakeCommentReaction.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var commentReaction = await CommentReactionRepository.FindByIdAsync(result.Value!.Id);

            commentReaction.Should().NotBeNull();

            commentReaction!.AssertCommentReaction(command, userId);

            var mongoEntity = await CommentReactionMongoRepository.FindByIdAsync(fakeCommentReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertCommentReactionMongoEntity(commentReaction!);

            result.Value.AssertCommentReactionDto(commentReaction);
        }

        [Test]
        public async Task Should_failure_while_updating_comment_reaction_when_user_is_not_authorized()
        {
            var fakePost = await CreateFakePost(Guid.NewGuid().ToString());

            var fakeComment = await CreateFakeComment(fakePost.Id, Guid.NewGuid().ToString());

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment.Id, Guid.NewGuid().ToString());

            var command = new UpdateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                ReactionId = fakeCommentReaction.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_while_updating_comment_reaction_when_user_dose_not_own_this_reaction()
        {
            UserService.Login();

            var fakePost = await CreateFakePost(Guid.NewGuid().ToString());

            var fakeComment = await CreateFakeComment(fakePost.Id, Guid.NewGuid().ToString());

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment.Id, Guid.NewGuid().ToString());

            var command = new UpdateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                ReactionId = fakeCommentReaction.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }


        private async Task<Comment> CreateFakeComment(string postId, string userId)
        {

            var comment = new Comment
            {
                Content = Guid.NewGuid().ToString(),
                UserId = userId,
                PostId = postId
            };

            return await CommentRepository.InsertAsync(comment);
        }

        private async Task<Post> CreateFakePost(string userId)
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                UserId = userId
            };

            return await PostRepository.InsertAsync(post);
        }

        private async Task<CommentReaction> CreateFakeCommentReaction(string commentId, string userId)
        {
            var commentReaction = new CommentReaction
            {
                CommentId = commentId,
                UserId = userId,
                Type = new Faker().PickRandom<ReactionType>()

            };

            return await CommentReactionRepository.InsertAsync(commentReaction);
        }
    }
}
