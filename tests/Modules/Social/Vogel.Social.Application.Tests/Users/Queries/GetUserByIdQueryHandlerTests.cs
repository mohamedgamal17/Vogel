using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Users.Queries.GetUserById;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class GetUserByIdQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get;  }
        public ISocialRepository<Picture> PictureRepository { get;  }

        public GetUserByIdQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            PictureRepository = ServiceProvider.GetRequiredService<ISocialRepository<Picture>>();
        }


        [Test]
        public async Task Should_get_user_Profile_by_id()
        {
            AuthenticationService.Login();

            var targetUser = await UserRepository.AsQuerable().PickRandom();

            var userPicture = await PictureRepository.FindByIdAsync(targetUser!.AvatarId ?? Guid.NewGuid().ToString());

            var query = new GetUserByIdQuery
            {
                Id = targetUser.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertUserDto(targetUser, userPicture);
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
