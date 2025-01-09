using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Pictures.Queries.GetPictureById;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Pictures.Queries
{
    public class GetPictureByIdQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<Picture> PictureRepository { get; }
        public ISocialRepository<User> UserRepository { get;  }
        public GetPictureByIdQueryHandlerTests()
        {
            PictureRepository = ServiceProvider.GetRequiredService<ISocialRepository<Picture>>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }


        [Test]
        public async Task Should_get_current_user_specific_picture_by_id()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            UserService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var targetPicture = await PictureRepository.AsQuerable().Where(x => x.UserId == currentUser.Id).PickRandom();

            var query = new GetPictureByIdQuery
            {
                Id = targetPicture!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertPictureDto(targetPicture);

        }

        [Test]
        public async Task Should_failure_while_getting_picture_by_id_when_picture_id_is_not_exist()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            UserService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetPictureByIdQuery
            {
                Id = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_picture_by_id_when_user_dose_not_own_picture()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            UserService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var targetPicture = await PictureRepository.AsQuerable().Where(x => x.UserId != currentUser.Id).PickRandom();

            var query = new GetPictureByIdQuery
            {
                Id = targetPicture!.Id
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));

        }

        [Test]
        public async Task Should_failure_while_getting_picture_by_id_when_user_is_not_authorized()
        {
            var query = new GetPictureByIdQuery
            {
                Id = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }


    }
}
