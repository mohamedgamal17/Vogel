using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.CommentReactions.Factories;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.CommentReactions;
namespace Vogel.Content.Application.CommentReactions.Commands.UpdateCommentReaction
{
    public class UpdateCommentReactionCommandHandler : IApplicationRequestHandler<UpdateCommentReactionCommand, CommentReactionDto>
    {
        private readonly IContentRepository<CommentReaction> _commentReactionRepository;
        private readonly IContentRepository<Comment> _commentRepository;
        private readonly IMongoRepository<CommentReactionMongoEntity> _commentReactionMongoRepository;
        private readonly ICommentReactionResponseFactory _commentReactionResponseFactory;
        private readonly ISecurityContext _securityContxt;

        public UpdateCommentReactionCommandHandler(IContentRepository<CommentReaction> commentReactionRepository, IContentRepository<Comment> commentRepository, IMongoRepository<CommentReactionMongoEntity> commentReactionMongoRepository, ICommentReactionResponseFactory commentReactionResponseFactory, ISecurityContext securityContxt)
        {
            _commentReactionRepository = commentReactionRepository;
            _commentRepository = commentRepository;
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _commentReactionResponseFactory = commentReactionResponseFactory;
            _securityContxt = securityContxt;
        }

        public async Task<Result<CommentReactionDto>> Handle(UpdateCommentReactionCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContxt.User!.Id;

            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var reaction = await _commentReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.CommentId == request.CommentId);

            if (reaction == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(CommentReaction), request.ReactionId));
            }

            if (!reaction.IsOwnedBy(userId))
            {
                return new Result<CommentReactionDto>(new ForbiddenAccessException());
            }

            reaction.Type = request.Type;

            await _commentReactionRepository.UpdateAsync(reaction);

            var reactionMongoEntity = await _commentReactionMongoRepository.FindByIdAsync(reaction.Id);

            return await _commentReactionResponseFactory.PrepareCommentReactionDto(reactionMongoEntity!);
        }
    }
}
