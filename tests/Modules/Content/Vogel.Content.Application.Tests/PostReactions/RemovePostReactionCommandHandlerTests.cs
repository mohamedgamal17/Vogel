using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Application.PostReactions.Commands.RemovePostReaction;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.Tests.PostReactions
{
    public class RemovePostReactionCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IContentRepository<PostReaction> PostReactionRepository { get; }
        public IMongoRepository<PostReactionMongoEntity> PostReactionMongoRepository { get; }


        public RemovePostReactionCommandHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<PostReaction>>();
            PostReactionMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PostReactionMongoEntity>>();
        }

        [Test]
        public async Task Should_delete_post_reaction()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            string userId = fakeUser!.Id;

            var fakePost = await CreateFakePost(userId);

            var fakePostReaction = await CreateFakePostReaction(fakePost.Id, userId);

            var command = new RemovePostReactionCommand
            {
                ReactionId = fakePostReaction.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var postReaction = await PostReactionRepository.FindByIdAsync(fakePostReaction.Id);

            postReaction.Should().BeNull();

            var mongoEntity = await PostReactionMongoRepository.FindByIdAsync(fakePostReaction.Id);

            mongoEntity.Should().BeNull();

        }

        [Test]
        public async Task Should_failure_while_deleting_post_when_user_is_not_authorized()
        {

            var fakePost = await CreateFakePost(Guid.NewGuid().ToString());

            var fakePostReaction = await CreateFakePostReaction(fakePost.Id, Guid.NewGuid().ToString());
     
            var command = new RemovePostReactionCommand
            {
                ReactionId = fakePostReaction.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Shoul_failure_while_deleting_post_when_user_is_not_own_post_reaction()
        {
            AuthenticationService.Login();

            var fakePost = await CreateFakePost(Guid.NewGuid().ToString());

            var fakePostReaction = await CreateFakePostReaction(fakePost.Id , Guid.NewGuid().ToString());
     
            var command = new RemovePostReactionCommand
            {
                ReactionId = fakePostReaction.Id,
                PostId = fakePost.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
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

        private async Task<PostReaction> CreateFakePostReaction(string postId, string userId)
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
