using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Application.Posts.Commands.CreatePost;
using Vogel.Content.Application.Tests.Fakers;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.Content.Application.Tests.Posts
{
    public class CreatePostCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get;  }
        public IMongoRepository<PostMongoEntity> PostMongoRepository { get; }
        public FakeMediaService MediaService { get; }

        public CreatePostCommandHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PostMongoEntity>>();
            MediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }


        [Test]
        public async Task Should_create_post()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            string userId = fakeUser!.Id;

            var fakeMedia = await CreateMediaAsync(userId);

            var command = new CreatePostCommand
            {
                MediaId = fakeMedia.Id,
                Caption = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var post = await PostRepository.FindByIdAsync(result.Value!.Id);

            post.Should().NotBeNull();

            var postMongoEntity = await PostMongoRepository.FindByIdAsync(post!.Id);

            postMongoEntity.Should().NotBeNull();

            post!.AssertPost(command,userId);

            post.AssertPostMongoEntity(postMongoEntity!);

            result.Value.AssertPostDto(post, fakeMedia);
        }

        [Test]
        public async Task Should_failure_while_creating_post_when_user_is_not_authorized()
        {
            var fakeMedia = await CreateMediaAsync(Guid.NewGuid().ToString());

            var command = new CreatePostCommand
            {
                MediaId = fakeMedia.Id,
                Caption = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_create_post_when_user_dose_not_own_this_media()
        {
            var fakeMedia = await CreateMediaAsync(Guid.NewGuid().ToString());

            AuthenticationService.Login();

            var command = new CreatePostCommand
            {
                MediaId = fakeMedia.Id,
                Caption = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        private Task<PublicMediaFileDto> CreateMediaAsync(string userId)
        {
            return Task.FromResult(MediaService.AddMedia(userId));
        }

    }
}
