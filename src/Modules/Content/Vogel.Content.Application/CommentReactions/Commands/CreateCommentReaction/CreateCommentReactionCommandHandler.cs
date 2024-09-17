using MongoDB.Driver;
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
namespace Vogel.Content.Application.CommentReactions.Commands.CreateCommentReaction
{
    public class CreateCommentReactionCommandHandler : IApplicationRequestHandler<CreateCommentReactionCommand, CommentReactionDto>
    {
        private readonly IContentRepository<CommentReaction> _commentReactionRepository;
        private readonly IContentRepository<Comment> _commentRepository;
        private readonly IMongoRepository<CommentReactionMongoEntity> _commentReactionMongoRepository;
        private readonly ICommentReactionResponseFactory _commentReactionResponseFactory;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public CreateCommentReactionCommandHandler(IContentRepository<CommentReaction> commentReactionRepository, IContentRepository<Comment> commentRepository, IMongoRepository<CommentReactionMongoEntity> commentReactionMongoRepository, ICommentReactionResponseFactory commentReactionResponseFactory, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentReactionRepository = commentReactionRepository;
            _commentRepository = commentRepository;
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _commentReactionResponseFactory = commentReactionResponseFactory;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<CommentReactionDto>> Handle(CreateCommentReactionCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var reaction = new CommentReaction
            {
                CommentId = comment.Id,
                UserId = _securityContext.User!.Id,
                Type = request.Type
            };

            await _commentReactionRepository.InsertAsync(reaction);

            var reactionView = await _commentReactionMongoRepository.SingleAsync(
                Builders<CommentReactionMongoEntity>.Filter.Eq(x => x.Id, reaction.Id)
               ); 
                 
            return await _commentReactionResponseFactory.PrepareCommentReactionDto(reactionView);
        }
    }
}
