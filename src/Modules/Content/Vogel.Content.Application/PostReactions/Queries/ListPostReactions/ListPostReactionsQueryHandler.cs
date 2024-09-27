using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.PostReactions.Factories;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Content.MongoEntities.Posts;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Extensions;
namespace Vogel.Content.Application.PostReactions.Queries.ListPostReactions
{
    public class ListPostReactionsQueryHandler : IApplicationRequestHandler<ListPostReactionsQuery, Paging<PostReactionDto>>
    {
        private readonly PostReactionMongoRepository _postReactionMongoRepository;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostReactionResponseFactory _postReactionResponseFactory;

        public ListPostReactionsQueryHandler(PostReactionMongoRepository postReactionMongoRepository, PostMongoRepository postMongoRepository, IPostReactionResponseFactory postReactionResponseFactory)
        {
            _postReactionMongoRepository = postReactionMongoRepository;
            _postMongoRepository = postMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
        }

        public async Task<Result<Paging<PostReactionDto>>> Handle(ListPostReactionsQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.FindByIdAsync(request.PostId);

            if(post == null)
            {
                return new Result<Paging<PostReactionDto>>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var paged = await _postReactionMongoRepository.AsMongoCollection()
                .Aggregate()
                .Match(x => x.PostId == request.PostId)
                .ToPaged(request.Cursor, request.Limit, request.Asending);


            var response = new Paging<PostReactionDto>
            {
                Data = await _postReactionResponseFactory.PrepareListPostReactionDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }
    }
}
