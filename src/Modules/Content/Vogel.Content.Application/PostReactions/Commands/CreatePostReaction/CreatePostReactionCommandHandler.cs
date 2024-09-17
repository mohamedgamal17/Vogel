using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.PostReactions.Factories;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.PostReactions.Commands.CreatePostReaction
{
    public class CreatePostReactionCommandHandler : IApplicationRequestHandler<CreatePostReactionCommand, PostReactionDto>
    {
        private readonly IContentRepository<PostReaction> _postReactionRepository;
        private readonly IContentRepository<Post> _postRepository;
        private readonly IMongoRepository<PostReactionMongoEntity> _postReactionMongoRepository;
        private readonly IPostReactionResponseFactory _postReactionResponseFactory;
        private readonly ISecurityContext _securityContext;

        public CreatePostReactionCommandHandler(IContentRepository<PostReaction> postReactionRepository, IContentRepository<Post> postRepository, IMongoRepository<PostReactionMongoEntity> postReactionMongoRepository, IPostReactionResponseFactory postReactionResponseFactory, ISecurityContext securityContext)
        {
            _postReactionRepository = postReactionRepository;
            _postRepository = postRepository;
            _postReactionMongoRepository = postReactionMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<PostReactionDto>> Handle(CreatePostReactionCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if (post == null)
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

            var mongoEntity = await _postReactionMongoRepository.FindByIdAsync(reaction.Id);

            return await _postReactionResponseFactory.PreparePostReactionDto(mongoEntity);
        }
    }
}
