using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.CommentReactions.Factories;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Content.MongoEntities.Posts;

namespace Vogel.Content.Application.CommentReactions.Queries.GetCommentReactionById
{
    public class GetCommentReactionByIdQueryHandler : IApplicationRequestHandler<GetCommentReactionByIdQuery, CommentReactionDto>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly CommentReactionMongoRepository _commentReactionMongoRepository;
        private readonly ICommentReactionResponseFactory _commentReactionResponseFactory;

        public GetCommentReactionByIdQueryHandler(PostMongoRepository postMongoRepository, CommentMongoRepository commentMongoRepository, CommentReactionMongoRepository commentReactionMongoRepository, ICommentReactionResponseFactory commentReactionResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _commentMongoRepository = commentMongoRepository;
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _commentReactionResponseFactory = commentReactionResponseFactory;
        }

        public async Task<Result<CommentReactionDto>> Handle(GetCommentReactionByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var comment = await _commentMongoRepository.AsQuerable()
                .SingleOrDefaultAsync(x => x.PostId == request.PostId && x.Id == request.CommentId);

            if (comment == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }


            var reaction = await _commentReactionMongoRepository.AsQuerable()
                .SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.CommentId == request.CommentId);

            if(reaction == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            return await _commentReactionResponseFactory.PrepareCommentReactionDto(reaction);
        }
    }
}
