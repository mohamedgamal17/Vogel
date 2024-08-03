using MediatR;
using Vogel.Application.Medias.Policies;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.Application.Posts.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Posts;
namespace Vogel.Application.Posts.Commands
{
    public class PostCommandHandler :
        IApplicationRequestHandler<CreatePostCommand, PostDto>,
        IApplicationRequestHandler<UpdatePostCommand, PostDto>,
        IApplicationRequestHandler<RemovePostCommand, Unit>
    {
        private readonly ISecurityContext _securityContext;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IPostResponseFactory _postResponseFactory;

        public PostCommandHandler(ISecurityContext securityContext, IRepository<Post> postRepository, IRepository<Media> mediaRepository, PostMongoRepository postMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IPostResponseFactory postResponseFactory)
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

            var postView = await _postMongoRepository.GetByIdPostMongoView(post.Id);

            return await _postResponseFactory.PreparePostDto(postView);
        }

        public async Task<Result<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {

            var post = await _postRepository.FindByIdAsync(request.Id);

            if (post == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.Id));
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

            var postView = await _postMongoRepository.GetByIdPostMongoView(post.Id);

            return await _postResponseFactory.PreparePostDto(postView);
        }

        public async Task<Result<Unit>> Handle(RemovePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.Id);

            if (post == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Post), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(post, PostOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }

            await _postRepository.DeleteAsync(post);

            return Unit.Value;
        }

    }
}
