using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Application.Comments.Commands.CreateComment;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Comments;
namespace Vogel.Content.Application.Tests.Comments
{
    public class CreateCommentHandlerTests : ContentTestFixture
    {
        public IContentRepository<Comment> CommentRepository { get;  }
        public IMongoRepository<CommentMongoEntity> CommentMongoRepository { get;  }
        public IContentRepository<Post> PostRepository { get; }

        public CreateCommentHandlerTests()
        {
            CommentRepository = ServiceProvider.GetRequiredService<IContentRepository<Comment>>();
            CommentMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<CommentMongoEntity>>();
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
        }


        [Test]
        public async Task Should_post_user_comment()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            string userId = fakeUser.Id;

            var fakePost = await CreatePostAsync(userId);

            var command = new CreateCommentCommand
            {
                Content = Guid.NewGuid().ToString(),
                PostId = fakePost.Id
            };


            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var comment = await  CommentRepository.FindByIdAsync(result.Value!.Id);

            comment.Should().NotBeNull();

            var commentMongoEntity = await CommentMongoRepository.FindByIdAsync(comment!.Id);

            commentMongoEntity.Should().NotBeNull();

            comment!.AssertComment(command, userId);

            comment.AssertCommentMongoEntity(commentMongoEntity!);

            result.Value.AssertCommentDto(comment);

        }


        [Test]
        public async Task Should_create_sub_comment()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            string userId = fakeUser!.Id;

            var fakePost = await CreatePostAsync(userId);

            var fakeComment = await CreateCommentAsync(fakePost.Id,userId);


            var command = new CreateCommentCommand
            {
                CommentId = fakeComment.Id,
                Content = Guid.NewGuid().ToString(),
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var comment = await CommentRepository.FindByIdAsync(result.Value!.Id);

            comment.Should().NotBeNull();

            var commentMongoEntity = await CommentMongoRepository.FindByIdAsync(comment!.Id);

            commentMongoEntity.Should().NotBeNull();

            comment!.AssertComment(command,userId);

            comment.AssertCommentMongoEntity(commentMongoEntity!);

            result.Value.AssertCommentDto(comment);
        }

        [Test]
        public async Task Should_return_failure_result_while_creating_comment_when_post_is_not_exist()
        {
            AuthenticationService.Login();

            var command = new CreateCommentCommand
            {
                Content = Guid.NewGuid().ToString(),
                PostId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }

        [Test]
        public async Task Should_return_failure_result_while_creating_comment_when_user_is_not_authorized()
        {
            var fakePost = await CreatePostAsync(Guid.NewGuid().ToString());

            var command = new CreateCommentCommand
            {
                Content = Guid.NewGuid().ToString(),
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
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

        private async Task<Comment> CreateCommentAsync(string postId , string userId , string? commentId = null)
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
