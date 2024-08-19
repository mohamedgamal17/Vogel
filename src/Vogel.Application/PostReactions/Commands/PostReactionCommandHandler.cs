using MediatR;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.PostReactions.Factories;
using Vogel.Application.PostReactions.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.PostReactions;
namespace Vogel.Application.PostReactions.Commands
{
    public class PostReactionCommandHandler :
        IApplicationRequestHandler<CreatePostReactionCommand, PostReactionDto>,
        IApplicationRequestHandler<UpdatePostReactionCommand, PostReactionDto>,
        IApplicationRequestHandler<RemovePostReactionCommand , Unit>
    {
        private readonly IRepository<PostReaction> _postReactionRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly PostReactionMongoRepository postReactionMongoRepository;
        private readonly IPostReactionResponseFactory _postReactionResponseFactory;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public PostReactionCommandHandler(IRepository<PostReaction> postReactionRepository, IRepository<Post> postRepository, PostReactionMongoRepository postReactionMongoRepository, IPostReactionResponseFactory postReactionResponseFactory, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _postReactionRepository = postReactionRepository;
            _postRepository = postRepository;
            this.postReactionMongoRepository = postReactionMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<PostReactionDto>> Handle(CreatePostReactionCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if(post == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var reaction = new PostReaction
            {
                PostId = post.Id,
                Type = request.Type,
                UserId = _securityContext.User!.Id,
            };

            await _postReactionRepository.InsertAsync(reaction);

            var mongoView = await postReactionMongoRepository
                .GetReactionViewById( reaction.PostId,reaction.Id);

            return await _postReactionResponseFactory.PreparePostReactionDto(mongoView!);
        }

        public async Task<Result<PostReactionDto>> Handle(UpdatePostReactionCommand request, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.PostId == request.PostId);

            if (reaction == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.ReactionId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, PostReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<PostReactionDto>(authorizationResult.Exception!);
            }

            reaction.Type = request.Type;

            await _postReactionRepository.UpdateAsync(reaction);

            var mongoView = await postReactionMongoRepository
                .GetReactionViewById(reaction.PostId,reaction.Id);

            return await _postReactionResponseFactory.PreparePostReactionDto(mongoView!);
        }

        public async Task<Result<Unit>> Handle(RemovePostReactionCommand request, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.PostId == request.PostId);

            if (reaction == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(PostReaction), request.ReactionId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, PostReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<Unit>(authorizationResult.Exception!);
            }

            await _postReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }

    }
}
