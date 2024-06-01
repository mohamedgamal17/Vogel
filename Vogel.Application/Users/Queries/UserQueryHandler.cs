using MongoDB.Driver;
using Vogel.Application.Common.Models;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.Users.Queries
{
    public class UserQueryHandler :
        IApplicationRequestHandler<ListUserQuery, Paging<UserDto>>,
        IApplicationRequestHandler<GetCurrentUserQuery, UserDto>,
        IApplicationRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly UserMongoViewRepository _userMongoViewRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;

        public UserQueryHandler(UserMongoViewRepository userMongoViewRepository, IUserResponseFactory userResponseFactory, ISecurityContext securityContext)
        {
            _userMongoViewRepository = userMongoViewRepository;
            _userResponseFactory = userResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<Paging<UserDto>>> Handle(ListUserQuery request, CancellationToken cancellationToken)
        {
            var query = _userMongoViewRepository.AsMongoCollection().Aggregate();

            query = SortQuery(query,request);

            var data = await Paginate(query, request);

            var prepareResponseTask = _userResponseFactory.PrepareListUserAggregateDto(data);

            var preaprePaginagInfoTask = PreparePagingInfo(query, request);

            await Task.WhenAll(prepareResponseTask, preaprePaginagInfoTask);

            var paged = new Paging<UserDto>
            {
                Data = prepareResponseTask.Result,
                Info = preaprePaginagInfoTask.Result
            };

            return paged;
        }
        private async Task<List<UserMongoView>> Paginate(IAggregateFluent<UserMongoView> query, ListUserQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<UserMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<UserMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }
        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<UserMongoView> query, ListUserQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<UserMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<UserMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<UserMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<UserMongoView>.Filter.Lt(x => x.Id, request.Cursor);

                var next = await query.Match(nextFilter).Skip(request.Limit - 1).FirstOrDefaultAsync();

                var previos = await query.Match(previosFilter).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, previos?.Id, request.Asending);
            }
            else
            {
                var next = await query.Skip(request.Limit - 1).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, null, request.Asending);
            }
        }

        private IAggregateFluent<UserMongoView> SortQuery(IAggregateFluent<UserMongoView> query , ListUserQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var user = await _userMongoViewRepository.FindByIdAsync(currentUserId);
     
            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(UserAggregate), currentUserId));
            }

            return await _userResponseFactory.PrepareUserAggregateDto(user);
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userMongoViewRepository.FindByIdAsync(request.Id);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(UserAggregate), request.Id));
            }

            return await _userResponseFactory.PrepareUserAggregateDto(user);
        }
    }
}
