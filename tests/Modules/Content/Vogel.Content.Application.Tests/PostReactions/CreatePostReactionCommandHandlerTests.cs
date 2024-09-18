using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Application.PostReactions.Commands.CreatePostReaction;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.Tests.PostReactions
{
    public class CreatePostReactionCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IContentRepository<PostReaction> PostReactionRepository { get;  }
        public IMongoRepository<PostReactionMongoEntity> PostReactionMongoRepository { get; }

        public CreatePostReactionCommandHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<PostReaction>>();
            PostReactionMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PostReactionMongoEntity>>();
        }

        [Test]
        public async Task Should_create_post_reaction()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var fakePost = await CreateFakePost(userId);

            var command = new CreatePostReactionCommand
            {
                Type = new Faker().PickRandom<Content.Domain.Common.ReactionType>(),
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var postReaction = await  PostReactionRepository.FindByIdAsync(result.Value!.Id);

            postReaction.Should().NotBeNull();

            postReaction!.AssertPostReaction(command, userId);

            var mongoEntity = await PostReactionMongoRepository.FindByIdAsync(postReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertPostReactionMongoEntity(postReaction);

            result.Value.AssertPostReactionDto(postReaction);

        }

        [Test]
        public async Task Should_failure_while_creating_post_reaction_when_user_is_not_authorized()
        {
            var fakePost = await CreateFakePost(Guid.NewGuid().ToString());

            var command = new CreatePostReactionCommand
            {
                Type = new Faker().PickRandom<Content.Domain.Common.ReactionType>(),
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        private async Task<Post> CreateFakePost(string userId)
        {
            var post = new Post
            {
                UserId = userId,
                Caption = Guid.NewGuid().ToString()
            };

            return await PostRepository.InsertAsync(post);
        }

        private async Task<PostReaction> CreateFakePostReaction(string postId , string userId)
        {
            var reaction = new PostReaction
            {
                UserId = userId,
                PostId = postId,
                Type = new Faker().PickRandom<Content.Domain.Common.ReactionType>()
            };

            return await PostReactionRepository.InsertAsync(reaction);
        }
    }
}
