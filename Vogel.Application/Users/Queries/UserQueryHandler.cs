using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
using Vogel.Domain.Utils;

namespace Vogel.Application.Users.Queries
{
    public class UserQueryHandler :
        IApplicationRequestHandler<ListUserQuery, Paging<UserAggregateDto>>,
        IApplicationRequestHandler<GetCurrentUserQuery, UserAggregateDto>,
        IApplicationRequestHandler<GetUserByIdQuery, UserAggregateDto>
    {
        private readonly IMongoDbRepository<User> _userRepository;
        private readonly IMongoDbRepository<Media> _mediaRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;

        public UserQueryHandler(IMongoDbRepository<User> userRepository, IMongoDbRepository<Media> mediaRepository, IUserResponseFactory userResponseFactory, ISecurityContext securityContext)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _userResponseFactory = userResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<Paging<UserAggregateDto>>> Handle(ListUserQuery request, CancellationToken cancellationToken)
        {
            var mediaCollection = _mediaRepository.AsMongoCollection();
            var query = _userRepository.AsMongoCollection()
                  .Aggregate()
                  .Lookup<User, Media, UserAggregate>(mediaCollection,
                      x => x.AvatarId,
                      y => y.Id,
                      x => x.Avatar
                  )
                  .Unwind<UserAggregate, UserAggregate>(x => x.Avatar, 
                  new AggregateUnwindOptions<UserAggregate> { PreserveNullAndEmptyArrays = true });

            query = SortQuery(query,request);

            var data = await Paginate(query, request);

            var prepareResponseTask = _userResponseFactory.PrepareListUserAggregateDto(data);

            var preaprePaginagInfoTask = PreparePagingInfo(query, request);

            await Task.WhenAll(prepareResponseTask, preaprePaginagInfoTask);

            var paged = new Paging<UserAggregateDto>
            {
                Data = prepareResponseTask.Result,
                Info = preaprePaginagInfoTask.Result
            };

            return paged;
        }
        private async Task<List<UserAggregate>> Paginate(IAggregateFluent<UserAggregate> query, ListUserQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<UserAggregate>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<UserAggregate>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }
        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<UserAggregate> query, ListUserQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<UserAggregate>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<UserAggregate>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<UserAggregate>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<UserAggregate>.Filter.Lt(x => x.Id, request.Cursor);

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

        private IAggregateFluent<UserAggregate> SortQuery(IAggregateFluent<UserAggregate> query , ListUserQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        public async Task<Result<UserAggregateDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var user = await _userRepository.FindByIdAsync(currentUserId);

            if(user == null)
            {
                return new Result<UserAggregateDto>(new EntityNotFoundException(typeof(User), currentUserId));
            }

            return await _userResponseFactory.PrepareUserAggregateDto(user);
        }

        public async Task<Result<UserAggregateDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(request.Id);

            if (user == null)
            {
                return new Result<UserAggregateDto>(new EntityNotFoundException(typeof(User), request.Id));
            }
            return await _userResponseFactory.PrepareUserAggregateDto(user);
        }
    }
}
