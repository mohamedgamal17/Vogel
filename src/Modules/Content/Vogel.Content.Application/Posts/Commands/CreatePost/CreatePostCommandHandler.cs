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

namespace Vogel.Content.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler : IApplicationRequestHandler<CreatePostCommand, PostDto>
    {
        private readonly ISecurityContext _securityContext;
        private readonly IContentRepository<Post> _postRepository;
        private readonly IMediaService _mediaService;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public CreatePostCommandHandler(ISecurityContext securityContext, IContentRepository<Post> postRepository, IMediaService mediaService, PostMongoRepository postMongoRepository, IPostResponseFactory postResponseFactory)
        {
            _securityContext = securityContext;
            _postRepository = postRepository;
            _mediaService = mediaService;
            _postMongoRepository = postMongoRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

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

            var post = new Post
            {
                UserId = _securityContext.User!.Id,
                Caption = request.Caption,
                MediaId = mediaId,
            };

            await _postRepository.InsertAsync(post);

            var postView = await _postMongoRepository.GetPostViewById(post.Id);

            return await _postResponseFactory.PreparePostDto(postView);
        }
    }
}
