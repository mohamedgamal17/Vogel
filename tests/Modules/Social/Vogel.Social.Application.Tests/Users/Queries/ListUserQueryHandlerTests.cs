using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
using Vogel.Social.Application.Users.Queries.ListUsers;

namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class ListUserQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public FakeMediaService FakeMediaService { get; }

        public ListUserQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }

        [Test]
        public async Task Should_return_paged_list_of_users()
        {
            AuthenticationService.Login();

            var targetUser = await UserRepository.AsQuerable().PickRandom();
            var avatar = FakeMediaService.AddMedia(targetUser!.Id);
            var cover = FakeMediaService.AddMedia(targetUser.Id);
            targetUser.AvatarId = avatar.Id;
            targetUser.CoverId = cover.Id;
            await UserRepository.UpdateAsync(targetUser);

            var query = new ListUsersQuery
            {
                Limit = 50
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().BeGreaterThan(0);

            var mappedTarget = result.Value.Data.FirstOrDefault(x => x.Id == targetUser.Id);
            mappedTarget.Should().NotBeNull();
            mappedTarget!.AvatarId.Should().Be(avatar.Id);
            mappedTarget.Avatar.Should().NotBeNull();
            mappedTarget.Avatar!.Id.Should().Be(avatar.Id);
            mappedTarget.CoverId.Should().Be(cover.Id);
            mappedTarget.Cover.Should().NotBeNull();
            mappedTarget.Cover!.Id.Should().Be(cover.Id);
        }

        [Test]
        public async Task Should_failure_while_listing_users_when_user_is_not_authorized()
        {
            var query = new ListUsersQuery
            {
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
