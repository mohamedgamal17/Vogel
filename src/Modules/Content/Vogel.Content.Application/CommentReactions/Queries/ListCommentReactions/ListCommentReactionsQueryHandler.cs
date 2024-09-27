using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.CommentReactions.Factories;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Content.MongoEntities.Posts;

namespace Vogel.Content.Application.CommentReactions.Queries.ListCommentReactions
{
    public class ListCommentReactionsQueryHandler : IApplicationRequestHandler<ListCommentReactionsQuery, Paging<CommentReactionDto>>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly CommentReactionMongoRepository _commentReactionMongoRepository;
        private readonly ICommentReactionResponseFactory _commentReactionResponseFactory;

        public ListCommentReactionsQueryHandler(PostMongoRepository postMongoRepository, CommentMongoRepository commentMongoRepository, CommentReactionMongoRepository commentReactionMongoRepository, ICommentReactionResponseFactory commentReactionResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _commentMongoRepository = commentMongoRepository;
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _commentReactionResponseFactory = commentReactionResponseFactory;
        }

        public async Task<Result<Paging<CommentReactionDto>>> Handle(ListCommentReactionsQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.FindByIdAsync(request.PostId);

            if(post == null)
            {
                return new Result<Paging<CommentReactionDto>>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var comment = await _commentMongoRepository.AsQuerable()
                    .SingleOrDefaultAsync(x => x.PostId == request.PostId && x.Id == request.CommentId);

            if (comment == null)
            {
                return new Result<Paging<CommentReactionDto>>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var paged = await _commentReactionMongoRepository
                .AsMongoCollection()
                .Aggregate()
                .Match(x => x.CommentId == request.CommentId)
                .ToPaged(request.Cursor, request.Limit, request.Asending);


            var response = new Paging<CommentReactionDto>
            {
                Data = await _commentReactionResponseFactory.PrepareListCommentReactionDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }
    }
}
