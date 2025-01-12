using Microsoft.Extensions.DependencyInjection;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Domain;
using Vogel.Content.MongoEntities.PostReactions;
using Bogus;
using Vogel.Content.Application.PostReactions.Commands.UpdatePostReaction;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.Content.Application.Tests.Extensions;

namespace Vogel.Content.Application.Tests.PostReactions
{
    public class UpdatePostReactionCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Post> PostRepository { get; }
        public IContentRepository<PostReaction> PostReactionRepository { get; }
        public IMongoRepository<PostReactionMongoEntity> PostReactionMongoRepository { get; }

        public UpdatePostReactionCommandHandlerTests()
        {
            PostRepository = ServiceProvider.GetRequiredService<IContentRepository<Post>>();
            PostReactionRepository = ServiceProvider.GetRequiredService<IContentRepository<PostReaction>>();
            PostReactionMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PostReactionMongoEntity>>();
        }

        [Test]
        public async Task Should_update_post_reaction()
        {
            var fakeUser = UserService.PickRandomUser()!;

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            string userId = fakeUser!.Id;

            var fakePost = await CreateFakePost(userId);

            var fakePostReaction = await CreateFakePostReaction(fakePost.Id, userId);

            var command = new UpdatePostReactionCommand
            {
                Type = new Faker().PickRandom<Domain.Common.ReactionType>(),
                PostId = fakePost.Id,
                ReactionId = fakePostReaction.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var postReaction = await PostReactionRepository.FindByIdAsync(result.Value!.Id);

            postReaction.Should().NotBeNull();

            postReaction!.AssertPostReaction(command,userId);

            var mongoEntity = await PostReactionMongoRepository.FindByIdAsync(postReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertPostReactionMongoEntity(postReaction);

            result.Value.AssertPostReactionDto(postReaction);

        }

        [Test]
        public async Task Should_failure_while_updating_post_reaction_when_user_is_not_authorized()
        {
           
            var fakePost = await CreateFakePost(Guid.NewGuid().ToString());

            var fakePostReaction = await CreateFakePostReaction(fakePost.Id, Guid.NewGuid().ToString());

            var command = new UpdatePostReactionCommand
            {
                Type = new Faker().PickRandom<Content.Domain.Common.ReactionType>(),
                PostId = fakePost.Id,
                ReactionId = fakePostReaction.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_while_updating_post_when_user_is_not_owned_reactiom()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var fakePost = await CreateFakePost(userId);

            var fakePostReaction = await CreateFakePostReaction(fakePost.Id, Guid.NewGuid().ToString());

            var command = new UpdatePostReactionCommand
            {
                Type = new Faker().PickRandom<Content.Domain.Common.ReactionType>(),
                PostId = fakePost.Id,
                ReactionId = fakePostReaction.Id

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
