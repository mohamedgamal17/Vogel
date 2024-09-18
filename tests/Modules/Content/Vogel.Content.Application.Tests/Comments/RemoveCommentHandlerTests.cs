using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Domain;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Content.Application.Comments.Commands.RemoveComent;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.BuildingBlocks.Domain.Exceptions;
namespace Vogel.Content.Application.Tests.Comments
{
    public class RemoveCommentHandlerTests : ContentTestFixture
    {
        public IContentRepository<Comment> CommentRepository { get; }
        public IMongoRepository<CommentMongoEntity> CommentMongoRepository { get; }
        public IContentRepository<Post> PostRepository { get; }

        public RemoveCommentHandlerTests()
        {
            CommentRepository = ServiceProvider.GetRequiredService<IContentRepository<Comment>>();
            CommentMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<CommentMongoEntity>>();
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
        }
        [Test]
        public async Task Should_remove_comment_when_comment_is_owned_to_the_user()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var fakePost = await CreatePostAsync(userId);

            var fakeComment = await CreateCommentAsync(fakePost.Id, userId);

            var command = new RemoveCommentCommand
            {
                CommentId = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var comment = await CommentRepository.FindByIdAsync(fakeComment.Id);

            comment.Should().BeNull();
        }

        [Test]
        public async Task Should_remove_comment_when_user_is_own_comment()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var fakePost = await CreatePostAsync(userId);

            var fakeComment = await CreateCommentAsync(fakePost.Id, userId);

            var command = new RemoveCommentCommand
            {
                CommentId = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var comment = await CommentRepository.FindByIdAsync(fakeComment.Id);

            comment.Should().BeNull();
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_user_is_not_authorized()
        {
            var fakePost = await CreatePostAsync(Guid.NewGuid().ToString());

            var fakeComment = await CreateCommentAsync(fakePost.Id , Guid.NewGuid().ToString());

            var command = new RemoveCommentCommand
            {
                CommentId = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_post_is_not_exist()
        {
            UserService.Login();

            var command = new RemoveCommentCommand
            {
                CommentId = Guid.NewGuid().ToString(),
                PostId = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_comment_is_not_exist()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var fakePost = await CreatePostAsync(userId);

            var command = new RemoveCommentCommand
            {
                CommentId = Guid.NewGuid().ToString(),
                PostId = fakePost.Id,
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_user_dose_not_own_comment()
        {
            UserService.Login();

            var fakePost = await CreatePostAsync(Guid.NewGuid().ToString());

            var fakeComment = await CreateCommentAsync(fakePost.Id, Guid.NewGuid().ToString());

            var command = new RemoveCommentCommand
            {
                CommentId= fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }



        private async Task<Post> CreatePostAsync(string userId)
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                UserId = userId

            };

            return await PostRepository.InsertAsync(post);
        }

        private async Task<Comment> CreateCommentAsync(string postId, string userId, string? commentId = null)
        {
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = Guid.NewGuid().ToString(),
                CommentId = commentId
            };

            return await CommentRepository.InsertAsync(comment);
        }
    }
}
