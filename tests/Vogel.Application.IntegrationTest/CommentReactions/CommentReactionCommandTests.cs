using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.CommentReactions.Commands;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Comments;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.CommentReactions;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.CommentReactions
{
    public class CommentReactionCommandTests
    {
        public CommentReactionMongoRepository CommentReactionMongoRepository { get; }

        public CommentReactionCommandTests()
        {
            CommentReactionMongoRepository = Testing.ServiceProvider.GetRequiredService<CommentReactionMongoRepository>();
        }

        [Test]
        public async Task Should_create_comment_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();


            var command = new CreateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var commentReaction = await FindByIdAsync<CommentReaction>(result.Value!.Id);

            commentReaction.Should().NotBeNull();

            commentReaction!.AssertCommentReactionCommand(command);

            var mongoEntity = await CommentReactionMongoRepository.FindByIdAsync(commentReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertCommentReactionMongoEntity(commentReaction);

            result.Value.AssertCommentReactionDto(commentReaction, CurrentUserProfile!);
        }


        [Test]
        public async Task Should_failure_while_creating_comment_reaction_while_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var fakePost = await CreateFakePost();

            var fakeComment = await CreateFakeComment(fakePost);


            var command = new CreateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_update_comment_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment);

            var command = new UpdateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                ReactionId = fakeCommentReaction.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var commentReaction = await FindByIdAsync<CommentReaction>(result.Value!.Id);

            commentReaction.Should().NotBeNull();

            commentReaction!.AssertCommentReactionCommand(command);

            var mongoEntity = await CommentReactionMongoRepository.FindByIdAsync(fakeCommentReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertCommentReactionMongoEntity(commentReaction!);

            result.Value.AssertCommentReactionDto(commentReaction, CurrentUserProfile!);
        }

        [Test]
        public async Task Should_failure_while_updating_comment_reaction_when_user_is_not_authorized()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment);

            RemoveCurrentUser();

            var command = new UpdateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId  =fakeComment.Id,
                ReactionId = fakeCommentReaction.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_while_updating_comment_reaction_when_user_dose_not_own_this_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment);

            await RunAsUserAsync();


            var command = new UpdateCommentReactionCommand
            {
                Type = new Faker().PickRandom<ReactionType>(),
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                ReactionId = fakeCommentReaction.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }


        [Test]
        public async Task Should_remove_comment_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment);

            var command = new RemoveCommentReactionCommand
            {
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                Id = fakeCommentReaction.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var commentReaction = await FindByIdAsync<CommentReaction>(fakeCommentReaction!.Id);

            commentReaction.Should().BeNull();

            var mongoEntity = await CommentReactionMongoRepository.FindByIdAsync(fakeCommentReaction!.Id);

            mongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_while_removing_comment_reaction_when_user_is_not_authorized()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment);

            RemoveCurrentUser();

            var command = new RemoveCommentReactionCommand
            {
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                Id = fakeCommentReaction.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_while_removing_comment_reaction_when_user_dose_not_own_comment_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakeComment = await CreateFakeComment(fakePost);

            await RunAsUserAsync();

            var fakeCommentReaction = await CreateFakeCommentReaction(fakeComment);

            await RunAsUserAsync();

            var command = new RemoveCommentReactionCommand
            {
                PostId = fakePost.Id,
                CommentId = fakeComment.Id,
                Id = fakeCommentReaction.Id
            };


            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));

        }


        private async Task<Post> CreateFakePost()
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                UserId = CurrentUserProfile?.Id ?? Guid.NewGuid().ToString()
            };

            return await InsertAsync(post);
        }

        private async Task<Comment> CreateFakeComment(Post post)
        {
            var comment = new Comment
            {
                Content = Guid.NewGuid().ToString(),
                UserId = CurrentUserProfile?.Id ?? Guid.NewGuid().ToString(),
                PostId = post.Id
            };

            return await InsertAsync(comment);
        }

        private async Task<CommentReaction> CreateFakeCommentReaction(Comment comment)
        {
            var commentReaction = new CommentReaction
            {
                CommentId = comment.Id,
                UserId = CurrentUserProfile?.Id ?? Guid.NewGuid().ToString(),
                Type = new Faker().PickRandom<ReactionType>()

            };

            return await InsertAsync(commentReaction);
        }

    }
}
