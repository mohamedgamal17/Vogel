using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Medias.Policies;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;

namespace Vogel.Content.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommandHandler : IApplicationRequestHandler<CreatePostCommand, PostDto>
    {
        private readonly ISecurityContext _securityContext;
        private readonly IContentRepository<Post> _postRepository;
        private readonly IContentRepository<Media> _mediaRepository;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IPostResponseFactory _postResponseFactory;

        public CreatePostCommandHandler(ISecurityContext securityContext, IContentRepository<Post> postRepository, IContentRepository<Media> mediaRepository, PostMongoRepository postMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IPostResponseFactory postResponseFactory)
        {
            _securityContext = securityContext;
            _postRepository = postRepository;
            _mediaRepository = mediaRepository;
            _postMongoRepository = postMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            Media? media = default(Media);


            if (request.MediaId != null)
            {
                media = await _mediaRepository.FindByIdAsync(request.MediaId);

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media!, MediaOperationRequirements.IsOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<PostDto>(authorizationResult.Exception!);
                }
            }
            var post = new Post
            {
                UserId = _securityContext.User!.Id,
                Caption = request.Caption,
                MediaId = media?.Id
            };

            await _postRepository.InsertAsync(post);

            var postView = await _postMongoRepository.GetPostViewById(post.Id);

            return await _postResponseFactory.PreparePostDto(postView);
        }
    }
}
