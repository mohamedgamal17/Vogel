using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Application.Users.Queries.GetCurrentUser;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class GetCurrentUserQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get;  }
        public FakeMediaService FakeMediaService { get; }

        public GetCurrentUserQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }

        [Test]
        public async Task Should_get_current_user_profile()
        {
            var targetUser = await UserRepository.AsQuerable().PickRandom();

            var userMedia = targetUser!.AvatarId == null ? null : FakeMediaService.AddMedia(targetUser.Id);
            if (userMedia != null)
            {
                targetUser.AvatarId = userMedia.Id;
                await UserRepository.UpdateAsync(targetUser);
            }

            AuthenticationService.Login(targetUser.Id, targetUser.FirstName + targetUser.LastName, new List<string>());

            var query = new GetCurrentUserQuery();

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertUserDto(targetUser, userMedia);
        }

        [Test]
        public async Task Should_failure_while_retriving_current_user_profile_when_user_dose_not_have_profile()
        {
            AuthenticationService.Login();

            var query = new GetCurrentUserQuery();

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_retriving_current_user_profile_when_user_is_not_authorized()
        {
            var query = new GetCurrentUserQuery();

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
