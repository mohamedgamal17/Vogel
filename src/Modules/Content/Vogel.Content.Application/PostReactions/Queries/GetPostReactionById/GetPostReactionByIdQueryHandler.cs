using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.PostReactions.Factories;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.PostReactions.Queries.GetPostReactionById
{
    public class GetPostReactionByIdQueryHandler : IApplicationRequestHandler<GetPostReactionByIdQuery, PostReactionDto>
    {
        private readonly PostReactionMongoRepository _postReactionMongoRepository;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostReactionResponseFactory _postReactionResponseFactory;

        public GetPostReactionByIdQueryHandler(PostReactionMongoRepository postReactionMongoRepository, PostMongoRepository postMongoRepository, IPostReactionResponseFactory postReactionResponseFactory)
        {
            _postReactionMongoRepository = postReactionMongoRepository;
            _postMongoRepository = postMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
        }

        public async Task<Result<PostReactionDto>> Handle(GetPostReactionByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.PostReactionId));
            }

            var reaction = await _postReactionMongoRepository.FindAsync(
                    Builders<PostReactionMongoEntity>.Filter.And(
                        Builders<PostReactionMongoEntity>.Filter.Eq(x => x.PostId, request.PostId),
                        Builders<PostReactionMongoEntity>.Filter.Eq(x => x.Id, request.PostReactionId)
                    )
                );

            if(reaction == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.PostReactionId));
            }

            return await _postReactionResponseFactory.PreparePostReactionDto(reaction);
        }
    }
}
