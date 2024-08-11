using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Application.PostReactions.Commands;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.PostReactions;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.PostReactions
{
    public class PostReactionCommandHandlerTests : BaseTestFixture
    {

        public PostReactionMongoRepository PostReactionMongoRepository { get; set; }

        public PostReactionCommandHandlerTests()
        {
            PostReactionMongoRepository = Testing.ServiceProvider.GetRequiredService<PostReactionMongoRepository>();
        }

        [Test]
        public async Task Should_create_post_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var command = new CreatePostReactionCommand
            {
                Type = new Faker().PickRandom<Domain.Posts.ReactionType>(),
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var postReaction = await FindByIdAsync<PostReaction>(result.Value!.Id);

            postReaction.Should().NotBeNull();

            postReaction!.AssertPostReaction(command);

            var mongoEntity = await PostReactionMongoRepository.FindByIdAsync(postReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertPostReactionMongoEntity(postReaction);

            result.Value.AssertPostReactionDto(postReaction, CurrentUserProfile!);
                 
        }

        [Test]
        public async Task Should_failure_while_creating_post_reaction_when_user_is_not_authorized()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            RemoveCurrentUser();

            var command = new CreatePostReactionCommand
            {
                Type = new Faker().PickRandom<Domain.Posts.ReactionType>(),
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_update_post_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            await RunAsUserAsync();

            var fakePostReaction = await CreateFakePostReaction(fakePost);

            var command = new UpdatePostReactionCommand
            {
                Type = new Faker().PickRandom<Domain.Posts.ReactionType>(),
                PostId = fakePost.Id,
                ReactionId = fakePostReaction.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var postReaction = await FindByIdAsync<PostReaction>(result.Value!.Id);

            postReaction.Should().NotBeNull();

            postReaction!.AssertPostReaction(command);

            var mongoEntity = await PostReactionMongoRepository.FindByIdAsync(postReaction!.Id);

            mongoEntity.Should().NotBeNull();

            mongoEntity!.AssertPostReactionMongoEntity(postReaction);

            result.Value.AssertPostReactionDto(postReaction, CurrentUserProfile!);

        }

        [Test]
        public async Task Should_failure_while_updating_post_reaction_when_user_is_not_authorized()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            var fakePostReaction = await CreateFakePostReaction(fakePost);

            RemoveCurrentUser();

            var command = new UpdatePostReactionCommand
            {
                Type = new Faker().PickRandom<Domain.Posts.ReactionType>(),
                PostId = fakePost.Id,
                ReactionId = fakePostReaction.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_while_updating_post_when_user_is_not_owned_post()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            var fakePostReaction = await CreateFakePostReaction(fakePost);

            await RunAsUserAsync();

            var command = new UpdatePostReactionCommand
            {
                Type = new Faker().PickRandom<Domain.Posts.ReactionType>(),
                PostId= fakePost.Id,
                ReactionId = fakePostReaction.Id

            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_delete_post_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            var fakePostReaction = await CreateFakePostReaction(fakePost);

            var command = new RemovePostReactionCommand
            {
                ReactionId = fakePostReaction.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var postReaction = await FindByIdAsync<PostReaction>(fakePostReaction.Id);

            postReaction.Should().BeNull();

            var mongoEntity = await PostReactionMongoRepository.FindByIdAsync(fakePostReaction.Id);

            mongoEntity.Should().BeNull();

        }

        [Test]
        public async Task Should_failure_while_deleting_post_when_user_is_not_authorized()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            var fakePostReaction = await CreateFakePostReaction(fakePost);

            RemoveCurrentUser();

            var command = new RemovePostReactionCommand
            {
                ReactionId = fakePostReaction.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Shoul_failure_while_deleting_post_when_user_is_not_own_post_reaction()
        {
            await RunAsUserAsync();

            var fakePost = await CreateFakePost();

            var fakePostReaction = await CreateFakePostReaction(fakePost);


            await RunAsUserAsync();


            var command = new RemovePostReactionCommand
            {
                ReactionId = fakePostReaction.Id,
                PostId = fakePost.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }


        private async Task<Post> CreateFakePost()
        {
            var post = new Post
            {
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString(),
                Caption = Guid.NewGuid().ToString()
            };

            return await InsertAsync(post);
        }

        private async Task<PostReaction> CreateFakePostReaction(Post post)
        {
            var reaction = new PostReaction
            {
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString(),
                PostId = post.Id,
                Type = new Faker().PickRandom<Domain.Posts.ReactionType>()
            };

            return await InsertAsync(reaction);
        }
    }
}
