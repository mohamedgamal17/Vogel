using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Users.Queries.GetCurrentUser;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class GetCurrentUserQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get;  }
        public ISocialRepository<Picture> PictureRepository { get;  }

        public GetCurrentUserQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            PictureRepository = ServiceProvider.GetRequiredService<ISocialRepository<Picture>>();
        }

        [Test]
        public async Task Should_get_current_user_profile()
        {
            var targetUser = await UserRepository.AsQuerable().PickRandom();

            var userPicture = await PictureRepository.FindByIdAsync(targetUser!.AvatarId ?? Guid.NewGuid().ToString());

            AuthenticationService.Login(targetUser.Id, targetUser.FirstName + targetUser.LastName, new List<string>());

            var query = new GetCurrentUserQuery();

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertUserDto(targetUser, userPicture);
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
