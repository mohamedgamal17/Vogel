using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Domain;
using Vogel.Content.MongoEntities.Posts;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Content.Application.Posts.Commands.RemovePost;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
namespace Vogel.Content.Application.Tests.Posts
{
    public class RemovePostCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IMongoRepository<PostMongoEntity> PostMongoRepository { get; }
        public RemovePostCommandHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PostMongoEntity>>();
        }

        [Test]
        public async Task Should_remove_post()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var fakePost = await CreatePostAsync(userId);

            var command = new RemovePostCommand
            {
                PostId = fakePost.Id,
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var post = await PostRepository.FindByIdAsync(fakePost.Id);

            post.Should().BeNull();

            var postMongoEntity = await PostMongoRepository.FindByIdAsync(command.PostId);

            postMongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_while_removing_post_when_user_is_not_authorized()
        {
            var fakePost = await CreatePostAsync(Guid.NewGuid().ToString());

            var command = new RemovePostCommand
            {
                PostId = fakePost.Id,
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
        private async Task<Post> CreatePostAsync(string userId, Media? media = null)
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                MediaId = media?.Id,
                UserId = userId,
            };

            return await PostRepository.InsertAsync(post);
        }
    }
}
