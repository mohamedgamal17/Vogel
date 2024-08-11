using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.PostReactions.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.PostReactions;
namespace Vogel.Application.PostReactions.Queries
{
    public class PostReactionQueryHandler :
        IApplicationRequestHandler<ListPostReactionQuery, Paging<PostReactionDto>>,
        IApplicationRequestHandler<GetPostReactionQuery, PostReactionDto>
    {
        private readonly PostReactionMongoRepository _postReactionMongoRepository;

        private readonly IPostReactionResponseFactory _postReactionResponseFactory;

        public PostReactionQueryHandler(PostReactionMongoRepository postReactionMongoRepository, IPostReactionResponseFactory postReactionResponseFactory)
        {
            _postReactionMongoRepository = postReactionMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
        }

        public async Task<Result<Paging<PostReactionDto>>> Handle(ListPostReactionQuery request, CancellationToken cancellationToken)
        {
            var paged = await _postReactionMongoRepository.GetReactionViewPaged(request.PostId, request.Cursor,
                request.Limit, request.Asending);

            var result = new Paging<PostReactionDto>
            {
                Data = await _postReactionResponseFactory.PreparePostReactionDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }

        public async Task<Result<PostReactionDto>> Handle(GetPostReactionQuery request, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionMongoRepository.GetReactionViewById(request.PostId, request.Id);

            if(reaction == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.Id));
            }

            return await _postReactionResponseFactory.PreparePostReactionDto(reaction);
        }

    }
}
