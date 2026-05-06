using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Application.Users.Queries.GetUserById;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class GetUserByIdQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get;  }
        public FakeMediaService FakeMediaService { get; }

        public GetUserByIdQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }


        [Test]
        public async Task Should_get_user_Profile_by_id()
        {
            AuthenticationService.Login();

            var targetUser = await UserRepository.AsQuerable().PickRandom();

            var avatar = FakeMediaService.AddMedia(targetUser!.Id);
            var cover = FakeMediaService.AddMedia(targetUser.Id);
            targetUser.AvatarId = avatar.Id;
            targetUser.CoverId = cover.Id;
            await UserRepository.UpdateAsync(targetUser);

            var query = new GetUserByIdQuery
            {
                Id = targetUser.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertUserDto(targetUser);
            result.Value.Avatar.Should().NotBeNull();
            result.Value.Avatar!.Id.Should().Be(avatar.Id);
            result.Value.Cover.Should().NotBeNull();
            result.Value.Cover!.Id.Should().Be(cover.Id);
        }

        [Test]
        public async Task Should_failure_while_getting_user_by_id_when_user_id_is_not_exist()
        {
            AuthenticationService.Login();

            var userId = Guid.NewGuid().ToString();

            var query = new GetUserByIdQuery
            {
                Id = userId
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }

        [Test]
        public async Task Should_failure_while_getting_user_by_id_when_user_is_not_authorized()
        {
            var userId = Guid.NewGuid().ToString();

            var query = new GetUserByIdQuery
            {
                Id = userId
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }
    }
}
