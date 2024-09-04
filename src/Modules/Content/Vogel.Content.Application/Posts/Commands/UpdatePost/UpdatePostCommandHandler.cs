using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Medias.Policies;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.Application.Posts.Policies;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;

namespace Vogel.Content.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommandHandler : IApplicationRequestHandler<UpdatePostCommand, PostDto>
    {
        private readonly IContentRepository<Post> _postRepository;
        private readonly IContentRepository<Media> _mediaRepository;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IPostResponseFactory _postResponseFactory;

        public UpdatePostCommandHandler(IContentRepository<Post> postRepository, IContentRepository<Media> mediaRepository, PostMongoRepository postMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IPostResponseFactory postResponseFactory)
        {
            _postRepository = postRepository;
            _mediaRepository = mediaRepository;
            _postMongoRepository = postMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(post, PostOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return new Result<PostDto>(authorizationResult.Exception!);
            }

            Media? media = default(Media);

            if (request.MediaId != null)
            {
                media = await _mediaRepository.FindByIdAsync(request.MediaId);

                var mediaAuthorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media!, MediaOperationRequirements.IsOwner);

                if (mediaAuthorizationResult.IsFailure)
                {
                    return new Result<PostDto>(mediaAuthorizationResult.Exception!);
                }
            }

            post.Caption = request.Caption;

            post.MediaId = media?.Id;

            await _postRepository.UpdateAsync(post);

            var postView = await _postMongoRepository.GetPostViewById(post.Id);

            return await _postResponseFactory.PreparePostDto(postView);
        }
    }
}
