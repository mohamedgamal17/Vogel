using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.SearchByName
{
    public class SearchUserByNameQueryHandler : IApplicationRequestHandler<SearchUserByNameQuery, Paging<UserDto>>
    {

        private readonly UserMongoRepository _userMongoRepository;
        private readonly PictureMongoRepository _pictureMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;

        public SearchUserByNameQueryHandler(UserMongoRepository userMongoRepository, PictureMongoRepository pictureMongoRepository, IUserResponseFactory userResponseFactory, ISecurityContext securityContext)
        {
            _userMongoRepository = userMongoRepository;
            _pictureMongoRepository = pictureMongoRepository;
            _userResponseFactory = userResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<Paging<UserDto>>> Handle(SearchUserByNameQuery request, CancellationToken cancellationToken)
        {
            string currenUserId = _securityContext.User!.Id;

            var result = await _userMongoRepository.AsMongoCollection()
                .Aggregate()

                .Match(Builders<UserMongoEntity>.Filter.Text(request.Name, new TextSearchOptions
                {
                    CaseSensitive = false,
                    DiacriticSensitive = false,
                }))
                .Match(Builders<UserMongoEntity>.Filter.Eq(x => x.Id, currenUserId))
                .Lookup<UserMongoEntity, PictureMongoEntity, UserMongoView>(
                    _pictureMongoRepository.AsMongoCollection(),
                    l => l.AvatarId,
                    f => f.Id,
                    r => r.Avatar
                )
                .Unwind(x => x.Avatar, new AggregateUnwindOptions<UserMongoView> { PreserveNullAndEmptyArrays = true })
                .ToPaged(request.Cursor, request.Limit, request.Asending);

            var paged = new Paging<UserDto>
            {
                Data = await _userResponseFactory.PrepareListUserDto(result.Data),
                Info = result.Info
            };

            return paged;
        }
    }
}
