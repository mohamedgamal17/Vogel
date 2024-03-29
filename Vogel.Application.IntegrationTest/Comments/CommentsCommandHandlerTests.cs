using Bogus;
using FluentAssertions;
using System.Security.Claims;
using Vogel.Application.Comments.Commands;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Domain;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Comments
{
    public class CommentsCommandHandlerTests
    {

        [Test]
        public async Task Should_post_user_comment()
        {
            await RunAsUserAsync();

            var fakePost = await CreatePostAsync();

            var command = new CreateCommentCommand
            {
                PostId = fakePost.Id,
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var comment = await FindByIdAsync<Comment>(result.Value!.Id);

            comment.Should().NotBeNull();

            comment!.AssertComment(command);
        }

        [Test]
        public async Task Should_return_failure_result_while_creating_comment_when_post_is_not_exist()
        {
            await RunAsUserAsync();

            var command = new CreateCommentCommand
            {
                PostId = Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<EntityNotFoundException>();
        }

        [Test]
        public async Task Should_return_failure_result_while_creating_comment_when_user_is_not_authorized()
        {
            var fakePost = await CreatePostAsync();

            var command = new CreateCommentCommand
            {
                PostId = fakePost.Id,
                Content = Guid.NewGuid().ToString()
            };

            RemoveCurrentUser();

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();
        }

        [Test]
        public async Task Should_update_comment()
        {
            await RunAsUserAsync();

            var fakePost = await CreatePostAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            var command = new UpdateCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id,
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var comment = await FindByIdAsync<Comment>(result.Value!.Id);

            comment.Should().NotBeNull();

            comment!.AssertComment(command);
        }



        [Test]
        public async Task Should_return_failure_result_while_updating_comment_when_user_is_not_authorized()
        {
            var fakePost = await CreatePostAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            RemoveCurrentUser();

            var command = new UpdateCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id,
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();
        }

        [Test]
        public async Task Should_return_failure_result_while_updating_comment_when_post_is_not_exist()
        {
            await RunAsUserAsync();

            var command = new UpdateCommentCommand
            {
                Id = Guid.NewGuid().ToString(),
                PostId = Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<EntityNotFoundException>();
        }

        [Test]
        public async Task Should_return_failure_result_while_updating_comment_when_comment_is_not_exist()
        {
            await RunAsUserAsync();
            var fakePost = await CreatePostAsync();
            var command = new UpdateCommentCommand
            {
                Id = Guid.NewGuid().ToString(),
                PostId = fakePost.Id,
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<EntityNotFoundException>();
        }

        [Test]
        public async Task Should_return_failure_result_while_updating_comment_when_user_is_dose_not_own_comment()
        {
            var fakePost = await CreatePostAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            await RunAsUserAsync();

            var command = new UpdateCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id,
                Content = Guid.NewGuid().ToString()
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<ForbiddenAccessException>();
        }

        [Test]
        public async Task Should_remove_comment_when_comment_is_owned_to_the_user()
        {
            var fakePost = await CreatePostAsync();

            await RunAsUserAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            var command = new RemoveCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var comment = await FindByIdAsync<Comment>(fakeComment.Id);

            comment.Should().BeNull();
        }

        [Test]
        public async Task Should_remove_comment_when_user_is_own_post()
        {          
            var fakePost = await CreatePostAsync();

            await RunAsUserAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            await RunAsUserAsync(fakePost.UserId);

            var command = new RemoveCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var comment = await FindByIdAsync<Comment>(fakeComment.Id);

            comment.Should().BeNull();
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_user_is_not_authorized()
        {
            var fakePost = await CreatePostAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            RemoveCurrentUser();

            var command = new RemoveCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();
        }


        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_post_is_not_exist()
        {
            await RunAsUserAsync();

            var command = new RemoveCommentCommand
            {
                Id = Guid.NewGuid().ToString(),
                PostId = Guid.NewGuid().ToString(),
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<EntityNotFoundException>();
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_comment_is_not_exist()
        {
            await RunAsUserAsync();
            var fakePost = await CreatePostAsync();
            var command = new RemoveCommentCommand
            {
                Id = Guid.NewGuid().ToString(),
                PostId = fakePost.Id,
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<EntityNotFoundException>();
        }

        [Test]
        public async Task Should_return_failure_result_while_removing_comment_when_user_dose_not_own_comment()
        {
            var fakePost = await CreatePostAsync();

            var fakeComment = await CreateCommentAsync(fakePost);

            await RunAsUserAsync();

            var command = new RemoveCommentCommand
            {
                Id = fakeComment.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<ForbiddenAccessException>();
        }



        private async Task<Post> CreatePostAsync()
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString()

            };

            return await InsertAsync(post);
        }

        private async Task<Comment> CreateCommentAsync(Post post)
        {
            var comment = new Comment
            {
                PostId = post.Id,
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString()
            };

            return await InsertAsync(comment);
        }
    }
}
