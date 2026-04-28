using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;
using Vogel.MediaEngine.Shared.Services;

namespace Vogel.Content.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommandHandler : IApplicationRequestHandler<UpdatePostCommand, PostDto>
    {
        private readonly IContentRepository<Post> _postRepository;
        private readonly IMediaService _mediaService;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;
        private readonly ISecurityContext _securityContext;

        public UpdatePostCommandHandler(IContentRepository<Post> postRepository, IMediaService mediaService, PostMongoRepository postMongoRepository, IPostResponseFactory postResponseFactory, ISecurityContext securityContext)
        {
            _postRepository = postRepository;
            _mediaService = mediaService;
            _postMongoRepository = postMongoRepository;
            _postResponseFactory = postResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var post = await _postRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            if (!post.IsOwnedBy(userId))
            {
                return new Result<PostDto>(new ForbiddenAccessException());
            }

            string? mediaId = null;

            if (request.MediaId != null)
            {
                var mediaResult = await _mediaService.GetMediaById(request.MediaId);
                if (mediaResult.IsFailure)
                {
                    return new Result<PostDto>(mediaResult.Exception!);
                }

                mediaId = mediaResult.Value!.Id;
            }

            post.Caption = request.Caption;

            post.MediaId = mediaId;

            await _postRepository.UpdateAsync(post);

            var postView = await _postMongoRepository.GetPostViewById(post.Id);

            return await _postResponseFactory.PreparePostDto(postView);
        }
    }
}
