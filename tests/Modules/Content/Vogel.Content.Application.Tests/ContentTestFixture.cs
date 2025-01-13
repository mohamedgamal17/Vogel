using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vogel.Application.Tests;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Infrastructure.Extensions;
using Vogel.Content.Application.Tests.Fakers;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Common;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Infrastructure.EntityFramework;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Content.Application.Tests
{
    [TestFixture]
    public class ContentTestFixture : TestFixture
    {
        protected FakeUserService UserService { get; }

        public ContentTestFixture()
        {
            UserService = ServiceProvider.GetRequiredService<FakeUserService>();
        }
        protected override Task SetupAsync(IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            services.InstallModule<ContentApplicationTestModuleInstaller>(configuration, hostEnvironment);
            return Task.CompletedTask;
        }
        protected override async Task InitializeAsync(IServiceProvider services)
        {
            await services.RunModulesBootstrapperAsync();

            await SeedData(services);
        }

        private async Task SeedData(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<ContentDbContext>();

            var userService = services.GetRequiredService<FakeUserService>();

            var users = await SeedUsers(userService);

            await SeedUsersFriends(userService, users);

            var medias = await SeedMedias(dbContext, users);

            var posts = await SeedPosts(dbContext, users, medias);

            var comments = await SeedComments(dbContext, userService, posts);

            await SeedPostReactions(dbContext, userService, posts);

            await SeedCommentReactions(dbContext, userService, comments);
        }

        private Task<List<UserDto>> SeedUsers(FakeUserService userService)
        {

            var users = new UserFaker().Generate(50);

            userService.AddRangeOfUsers(users);

            return Task.FromResult(users);
        }

        private Task<Dictionary<string , List<UserDto>>> SeedUsersFriends(FakeUserService userService, List<UserDto> users)
        {

            Dictionary<string, List<UserDto>> data = new Dictionary<string, List<UserDto>>();

            foreach (var user in users)
            {
                var friends = users.Where(x => x.Id != user.Id).PickRandom(12).ToList();

                userService.AddUserFriends(user.Id,friends);

                data[user.Id] = friends;
            }

            return Task.FromResult(data);
                ;
        }

        private async Task<List<Media>> SeedMedias(ContentDbContext dbContext, List<UserDto> users)
        {
            List<Media> medias = new List<Media>();

            foreach (var user in users)
            {
                var userMedias = new MediaFaker(user.Id).Generate(3);

                await dbContext.AddRangeAsync(userMedias);

                medias.AddRange(userMedias);
            }

            await dbContext.SaveChangesAsync();

            return medias;
        }
        private async Task<List<Post>> SeedPosts(ContentDbContext dbContext, List<UserDto> users , List<Media> medias)
        {
            List<Post> posts = new List<Post>();

            var faker = new Faker();

            foreach (var user in users)
            {

                bool hasMedia = faker.Random.Bool();

                string? mediaId = hasMedia ? medias.Where(x => x.UserId == user.Id).PickRandom()!.Id : null;

                var userPosts = new PostFaker(user.Id, mediaId).Generate(10);

                await dbContext.AddRangeAsync(userPosts);

                posts.AddRange(posts);
            }

            await dbContext.SaveChangesAsync();
            return posts;
        }

        private async Task<List<Comment>> SeedComments(ContentDbContext dbContext, FakeUserService userService , List<Post> posts)
        {
            List<Comment> comments = new List<Comment>();

            var faker = new Faker();

            foreach (var post in posts)
            {
                var userFriends = userService.PickRandomUserFriend(post.UserId, 10).Select(x => x.Id).ToList();

                var postComments = new CommentFaker(userFriends, post.Id).Generate(10);

                await dbContext.AddRangeAsync(postComments);

                comments.AddRange(postComments);

                foreach (var item in postComments)
                {
                    bool hasSubComment = faker.Random.Bool();

                    if (hasSubComment)
                    {
                        var subComments = new CommentFaker(userFriends, item).Generate(5);

                        await dbContext.AddRangeAsync(subComments);

                        comments.AddRange(subComments);
                    }
                }
            }

            await dbContext.SaveChangesAsync();

            return comments;
        }

        private async Task SeedPostReactions(ContentDbContext dbContext , FakeUserService userService , List<Post> posts)
        {
            List<PostReaction> reactions = new List<PostReaction>();

            var faker = new Faker();

            foreach (var post in posts)
            {
                var users = userService.PickRandomUser(10);

                var postReactions = users!.Select(x => new PostReaction
                {
                    UserId = x.Id,
                    PostId = post.Id,
                    Type = faker.PickRandom<ReactionType>()
                }).ToList();

                reactions.AddRange(postReactions);

                await dbContext.AddRangeAsync(reactions);
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task SeedCommentReactions(ContentDbContext dbContext , FakeUserService userService , List<Comment> comments)
        {
            List<CommentReaction> reactions = new List<CommentReaction>();

            var faker = new Faker();

            foreach (var comment in comments)
            {
                var users = userService.PickRandomUser(10);

                var commentReactions = users!.Select(x => new CommentReaction
                {
                    UserId = x.Id,
                    CommentId = comment.Id,
                    Type = faker.PickRandom<ReactionType>()
                }).ToList();


                reactions.AddRange(reactions);

                await dbContext.AddRangeAsync(reactions);
            }

        }
        protected override async Task ShutdownAsync(IServiceProvider services)
        {
            await DropSqlDb();
            await DropMongoDb();
        }
    }
}
