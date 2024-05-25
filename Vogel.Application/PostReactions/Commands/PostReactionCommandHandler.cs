using MediatR;
using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Domain.Posts;
using Vogel.Domain.Utils;
namespace Vogel.Application.PostReactions.Commands
{
    public class PostReactionCommandHandler :
        IApplicationRequestHandler<CreatePostReactionCommand, PostReactionDto>,
        IApplicationRequestHandler<UpdatePostReactionCommand, PostReactionDto>,
        IApplicationRequestHandler<RemovePostReactionCommand , Unit>
    {
        private readonly IMongoDbRepository<PostReaction> _postReactionRepository;

        private readonly IMongoDbRepository<Post> _postRepository;

        private readonly ISecurityContext _securityContext;
        public PostReactionCommandHandler(IMongoDbRepository<PostReaction> postReactionRepository, IMongoDbRepository<Post> postRepository, ISecurityContext securityContext)
        {
            _postReactionRepository = postReactionRepository;
            _postRepository = postRepository;
            _securityContext = securityContext;
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

            return PreparePostReactionDto(reaction);
        }

        public async Task<Result<PostReactionDto>> Handle(UpdatePostReactionCommand request, CancellationToken cancellationToken)
        {
            var filter = new FilterDefinitionBuilder<PostReaction>()
                .And(
                    new FilterDefinitionBuilder<PostReaction>().Eq(x => x.PostId, request.PostId),
                    new FilterDefinitionBuilder<PostReaction>().Eq(x => x.Id, request.Id)
                 );

            var reaction = await _postReactionRepository.FindAsync(filter);

            if(reaction == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.Id));
            }

            reaction.Type = request.Type;

            await _postReactionRepository.UpdateAsync(reaction);

            return PreparePostReactionDto(reaction);
        }

        public async Task<Result<Unit>> Handle(RemovePostReactionCommand request, CancellationToken cancellationToken)
        {
            var filter = new FilterDefinitionBuilder<PostReaction>()
             .And(
                 new FilterDefinitionBuilder<PostReaction>().Eq(x => x.PostId, request.PostId),
                 new FilterDefinitionBuilder<PostReaction>().Eq(x => x.Id, request.Id)
              );

            var reaction = await _postReactionRepository.FindAsync(filter);


            if (reaction == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(PostReaction), request.Id));
            }

            await _postReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }

        private PostReactionDto PreparePostReactionDto(PostReaction postReaction)
        {
            var dto = new PostReactionDto
            {
                Id = postReaction.Id,
                PostId = postReaction.PostId,
                UserId = postReaction.UserId,
                Type = postReaction.Type
            };

            return dto;
        }
    }
}
