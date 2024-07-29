using MediatR;
using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.CommentReactions.Factories;
using Vogel.Application.CommentReactions.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Comments;
using Vogel.MongoDb.Entities.CommentReactions;

namespace Vogel.Application.CommentReactions.Commands
{
    public class CommentReactionCommandHandler : 
        IApplicationRequestHandler<CreateCommentReactionCommand, CommentReactionDto>,
        IApplicationRequestHandler<UpdateCommentReactionCommand, CommentReactionDto>,
        IApplicationRequestHandler<RemoveCommentReactionCommand,  Unit>
    {
        private readonly IRepository<CommentReaction> _commentReactionRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly CommentReactionMongoViewRepository _commentReactionMongoViewReposiotry;
        private readonly ICommentReactionResponseFactory _commentReactionResponseFactory;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public CommentReactionCommandHandler(IRepository<CommentReaction> commentReactionRepository, IRepository<Comment> commentRepository, CommentReactionMongoViewRepository commentReactionMongoViewReposiotry, ICommentReactionResponseFactory commentReactionResponseFactory, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentReactionRepository = commentReactionRepository;
            _commentRepository = commentRepository;
            _commentReactionMongoViewReposiotry = commentReactionMongoViewReposiotry;
            _commentReactionResponseFactory = commentReactionResponseFactory;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<CommentReactionDto>> Handle(CreateCommentReactionCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if(comment == null)
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

            var mongoEntity = await _commentReactionMongoViewReposiotry.FindByIdAsync(reaction.Id);

            return await _commentReactionResponseFactory.PrepareCommentReactionDto(mongoEntity!);
        }

        public async Task<Result<CommentReactionDto>> Handle(UpdateCommentReactionCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var reaction = await _commentReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.CommentId == request.CommentId);

            if(reaction == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(CommentReaction), request.ReactionId));
            }


            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, CommentReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<CommentReactionDto>(authorizationResult.Exception!);
            }

            reaction.Type = request.Type;

            await _commentReactionRepository.UpdateAsync(reaction);

            var mongoEntity = await _commentReactionMongoViewReposiotry.FindByIdAsync(reaction.Id);

            return await _commentReactionResponseFactory.PrepareCommentReactionDto(mongoEntity!);

        }

        public async Task<Result<Unit>> Handle(RemoveCommentReactionCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var reaction = await _commentReactionRepository.SingleOrDefaultAsync(x => x.Id == request.Id && x.CommentId == request.CommentId);

            if (reaction == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(CommentReaction), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, CommentReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<Unit>(authorizationResult.Exception!);
            }

            await _commentReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }
    }
}
