using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Pictures.Queries.ListCurrentUserPictures;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Pictures.Queries
{
    public class ListPictureQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<Picture> PictureRepository { get; }
        public ISocialRepository<User> UserRepository { get; }
        public GetPictureByIdQueryHandlerTests()
        {
            PictureRepository = ServiceProvider.GetRequiredService<ISocialRepository<Picture>>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_list_current_user_pictures()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            UserService.Login(currentUser.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new ListCurrentUserPicturesQuery
            {
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.UserId == currentUser.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_current_user_pictures_when_user_is_not_authorized()
        {
            var query = new ListCurrentUserPicturesQuery
            {
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
