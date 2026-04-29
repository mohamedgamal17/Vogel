using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.SearchByName
{
    public class SearchUserByNameQueryHandler : IApplicationRequestHandler<SearchUserByNameQuery, Paging<UserDto>>
    {

        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;

        public SearchUserByNameQueryHandler(UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, ISecurityContext securityContext)
        {
            _userMongoRepository = userMongoRepository;
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
                .ToPaged(request.Cursor, request.Limit, request.Asending);

            var paged = new Paging<UserDto>
            {
                Data = await _userResponseFactory.PrepareListUserDto(result.Data.Select(x => new UserMongoView
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    AvatarId = x.AvatarId,
                    CreationTime = x.CreationTime,
                    CreatorId = x.CreatorId,
                    ModificationTime = x.ModificationTime,
                    ModifierId = x.ModifierId
                }).ToList()),
                Info = result.Info
            };

            return paged;
        }
    }
}
