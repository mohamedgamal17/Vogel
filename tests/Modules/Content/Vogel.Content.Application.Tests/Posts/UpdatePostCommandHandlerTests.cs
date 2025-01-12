using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Domain;
using Vogel.Content.MongoEntities.Posts;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Content.Application.Posts.Commands.UpdatePost;
using FluentAssertions;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;

namespace Vogel.Content.Application.Tests.Posts
{
    public class UpdatePostCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IMongoRepository<PostMongoEntity> PostMongoRepository { get; }
        public IContentRepository<Media> MediaRepository { get; }

        public UpdatePostCommandHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PostMongoEntity>>();
            MediaRepository = ServiceProvider.GetRequiredService<IContentRepository<Media>>();
        }

        [Test]
        public async Task Should_update_post()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var fakeMedia = await CreateMediaAsync(userId);

            var fakePost = await CreatePostAsync(userId, fakeMedia);

            var fakeMedia1 = await CreateMediaAsync(userId);

            var command = new UpdatePostCommand
            {
                PostId = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia1.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var post = await PostRepository.FindByIdAsync(result.Value!.Id);

            post.Should().NotBeNull();

            var postMongoEntity = await PostMongoRepository.FindByIdAsync(post!.Id);

            postMongoEntity.Should().NotBeNull();

            post!.AssertPost(command, userId);

            post.AssertPostMongoEntity(postMongoEntity!);

            result.Value.AssertPostDto(post, fakeMedia1);
        }


        [Test]
        public async Task Should_failure_while_updaing_post_when_user_is_not_authorized()
        {
            var fakeMedia = await CreateMediaAsync(Guid.NewGuid().ToString());

            var fakePost = await CreatePostAsync(Guid.NewGuid().ToString() ,  fakeMedia);

            var command = new UpdatePostCommand
            {
                PostId = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_updating_post_when_user_dose_not_own_this_post()
        {
            var fakePost = await CreatePostAsync(Guid.NewGuid().ToString());

            AuthenticationService.Login();

            var command = new UpdatePostCommand
            {
                PostId = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }


        [Test]
        public async Task Should_failure_while_updating_post_when_user_dose_not_own_this_media()
        {
            var fakeMedia = await CreateMediaAsync(Guid.NewGuid().ToString());

            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var fakePost = await CreatePostAsync(userId);

            var command = new UpdatePostCommand
            {
                PostId = fakePost.Id,
                Caption = Guid.NewGuid().ToString(),
                MediaId = fakeMedia.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }


        private async Task<Post> CreatePostAsync(string userId , Media? media = null)
        {
            var post = new Post
            {
                Caption = Guid.NewGuid().ToString(),
                MediaId = media?.Id,
                UserId = userId,

            };

            return await PostRepository.InsertAsync(post);
        }

        private async Task<Media> CreateMediaAsync(string userId)
        {
            var media = new Media()
            {
                MediaType = Domain.Medias.MediaType.Image,
                Size = 56666,
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                UserId = userId
            };

            return await MediaRepository.InsertAsync(media);
        }
    }
}
