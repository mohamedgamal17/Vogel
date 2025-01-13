using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Content.Application.Medias.Queries.GetMediaById;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;

namespace Vogel.Content.Application.Tests.Medias.Queries
{
    public class GetMediaByIdQueryHandlerTests : ContentTestFixture
    {   
        public IContentRepository<Media> MediaRepository { get; }
        public GetMediaByIdQueryHandlerTests()
        {
            MediaRepository = ServiceProvider.GetRequiredService<IContentRepository<Media>>();
        }

        [Test]
        public async Task Should_get_current_user_media_by_id()
        {
            var fakeUser = UserService.PickRandomUser()!;

            var fakeMedia = await MediaRepository.AsQuerable().Where(x => x.UserId == fakeUser.Id).PickRandom();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetMediaByIdQuery
            {
                MediaId = fakeMedia!.Id
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertMediaDto(fakeMedia);
        }

        [Test]
        public async Task Should_failure_while_getting_curent_user_media_by_id_when_media_id_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new GetMediaByIdQuery
            {
                MediaId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }

        [Test]
        public async Task Should_failure_while_getting_current_user_media_by_id_when_user_dose_not_own_this_media()
        {
            var fakeUser = UserService.PickRandomUser()!;

            var fakeMedia = await MediaRepository.AsQuerable().Where(x => x.UserId != fakeUser.Id).PickRandom();

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new GetMediaByIdQuery
            {
                MediaId = fakeMedia!.Id
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_getting_curent_user_media_by_id_when_user_is_not_authorized()
        {
            var query = new GetMediaByIdQuery
            {
                MediaId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}
