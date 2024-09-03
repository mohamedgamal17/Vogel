using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.ListUsersByIds
{
    public class ListUsersByIdsQuerHandler : IApplicationRequestHandler<ListUsersByIdsQuery, Paging<UserDto>>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;

        public ListUsersByIdsQuerHandler(UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory)
        {
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
        }

        public async Task<Result<Paging<UserDto>>> Handle(ListUsersByIdsQuery request, CancellationToken cancellationToken)
        {
            var query = _userMongoRepository.GetUserAsAggregate();

            if (request.Ids != null)
            {
                query = query.Match(Builders<UserMongoView>.Filter.In(x => x.Id, request.Ids));
            }

            var paged = await query.ToPaged(request.Cursor, request.Limit, request.Asending);

            var result = new Paging<UserDto>
            {
                Data = await _userResponseFactory.PrepareListUserDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }
    }
}
