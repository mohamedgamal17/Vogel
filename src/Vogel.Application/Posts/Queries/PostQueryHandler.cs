using MongoDB.Driver;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Posts;
using Vogel.MongoDb.Entities.Extensions;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Shared.Results;
namespace Vogel.Application.Posts.Queries
{
    public class PostQueryHandler :
        IApplicationRequestHandler<ListPostQuery, Paging<PostDto>>,
        IApplicationRequestHandler<ListUserPostQuery, Paging<PostDto>>,
        IApplicationRequestHandler<GetUserPostById, PostDto>,
        IApplicationRequestHandler<GetPostByIdQuery, PostDto>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;
        private readonly ISecurityContext _securityContext;

        public PostQueryHandler(PostMongoRepository postMongoRepository, IPostResponseFactory postResponseFactory, ISecurityContext securityContext)
        {
            _postMongoRepository = postMongoRepository;
            _postResponseFactory = postResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<Paging<PostDto>>> Handle(ListPostQuery request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var paged = await _postMongoRepository.GetUserFriendsPosts(userId, request.Cursor, request.Limit, request.Asending);

            var result = new Paging<PostDto>
            {
                Data =await _postResponseFactory.PrepareListPostDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }

        private IAggregateFluent<PostMongoView> SortQuery(IAggregateFluent<PostMongoView> query, ListPostQueryBase request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _postMongoRepository.GetByIdPostMongoView(request.Id);

            if (result == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }


            return await _postResponseFactory.PreparePostDto(result);
        }

        public async Task<Result<Paging<PostDto>>> Handle(ListUserPostQuery request, CancellationToken cancellationToken)
        {
            var paged = await _postMongoRepository.GetPostAsAggregate()
                        .Match(x=> x.UserId == request.UserId)
                        .ToPaged(request.Cursor,request.Limit,request.Asending);
                        
            var result = new Paging<PostDto>
            {
                Data = await _postResponseFactory.PrepareListPostDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }

        public async Task<Result<PostDto>> Handle(GetUserPostById request, CancellationToken cancellationToken)
        {
            var result = await _postMongoRepository.GetPostAsAggregate()
                      .Match(x => x.UserId == request.UserId && x.Id == request.Id)
                      .SingleOrDefaultAsync();

            if (result == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }

            return await _postResponseFactory.PreparePostDto(result);
        }

    
    }
}
