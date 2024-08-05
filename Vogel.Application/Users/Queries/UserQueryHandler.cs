using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Users;
using MongoDB.Driver;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.Extensions;
namespace Vogel.Application.Users.Queries
{
    public class UserQueryHandler :
        IApplicationRequestHandler<ListUserQuery, Paging<UserDto>>,
        IApplicationRequestHandler<GetCurrentUserQuery, UserDto>,
        IApplicationRequestHandler<GetUserByIdQuery, UserDto>,
        IApplicationRequestHandler<SearchOnUserByNameQuery, Paging<UserDto>>
    {
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;
        private readonly UserMongoRepository _userMongoRepository;
        private readonly MediaMongoRepository _mediaMongoRepository;

        public UserQueryHandler(IUserResponseFactory userResponseFactory, ISecurityContext securityContext, UserMongoRepository userMongoRepository, MediaMongoRepository mediaMongoRepository)
        {
            _userResponseFactory = userResponseFactory;
            _securityContext = securityContext;
            _userMongoRepository = userMongoRepository;
            _mediaMongoRepository = mediaMongoRepository;
        }

        public async Task<Result<Paging<UserDto>>> Handle(ListUserQuery request, CancellationToken cancellationToken)
        {
            var paged = await _userMongoRepository.GetUserViewPaged(request.Cursor, request.Asending, request.Limit);

            var result = new Paging<UserDto>
            {
                Data = await _userResponseFactory.PrepareListUserDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }

        public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var user = await _userMongoRepository.GetByIdUserMongoView(currentUserId);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), currentUserId));
            }

            return await _userResponseFactory.PrepareUserDto(user);
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userMongoRepository.GetByIdUserMongoView(request.Id);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), request.Id));
            }

            return await _userResponseFactory.PrepareUserDto(user);
        }

        public async Task<Result<Paging<UserDto>>> Handle(SearchOnUserByNameQuery request, CancellationToken cancellationToken)
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
                .Lookup<UserMongoEntity, MediaMongoEntity, UserMongoView>(
                    _mediaMongoRepository.AsMongoCollection(),
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
