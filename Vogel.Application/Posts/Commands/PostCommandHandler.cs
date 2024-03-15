using MediatR;
using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Policies;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.Application.Posts.Policies;
using Vogel.Domain;
using Vogel.Domain.Utils;

namespace Vogel.Application.Posts.Commands
{
    public class PostCommandHandler : 
        IApplicationRequestHandler<CreatePostCommand, PostAggregateDto>,
        IApplicationRequestHandler<UpdatePostCommand , PostAggregateDto>,
        IApplicationRequestHandler<RemovePostCommand , Unit>
    {
        private readonly ISecurityContext _securityContext;
        private readonly IMongoDbRepository<Post> _postRepository;
        private readonly IMongoDbRepository<Media> _mediaRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IPostResponseFactory _postResponseFactory;

        public PostCommandHandler(ISecurityContext securityContext, IMongoDbRepository<Post> postRepository, IMongoDbRepository<Media> mediaRepository, IApplicationAuthorizationService applicationAuthorizationService, IPostResponseFactory postResponseFactory)
        {
            _securityContext = securityContext;
            _postRepository = postRepository;
            _mediaRepository = mediaRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<PostAggregateDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            Media? media = default(Media);

            if(request.MediaId != null)
            {
                media = await _mediaRepository.SingleAsync(new FilterDefinitionBuilder<Media>().Eq(x => x.Id, request.MediaId));

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<PostAggregateDto>(authorizationResult.Exception!);
                }
            }

            var post = new Post
            {
                UserId = _securityContext.User!.Id,
                Caption = request.Caption,
                MediaId = media?.Id
            };

            post = await _postRepository.InsertAsync(post);

            return await _postResponseFactory.PreparePostAggregateDto(post);
        }

        public async Task<Result<PostAggregateDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {

            var post = await _postRepository.FindByIdAsync(request.Id);

            if(post == null)
            {
                return new Result<PostAggregateDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(post , PostOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return new Result<PostAggregateDto>(authorizationResult.Exception!);
            }

            Media? media = default(Media);

            if (request.MediaId != null)
            {
                media = await _mediaRepository.SingleAsync(new FilterDefinitionBuilder<Media>().Eq(x => x.Id, request.MediaId));

                var mediaAuthorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

                if (mediaAuthorizationResult.IsFailure)
                {
                    return new Result<PostAggregateDto>(mediaAuthorizationResult.Exception!);
                }
            }

            post.Caption = request.Caption;

            post.MediaId = media?.Id;

            await _postRepository.UpdateAsync(post);

            return await _postResponseFactory.PreparePostAggregateDto(post);
        }

        public async Task<Result<Unit>> Handle(RemovePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.Id);

            if(post == null)
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

        private PostDto PreparePostDto(Post post)
        {
            var dto = new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Caption = post.Caption,
                MediaId = post.MediaId
            };

            return dto;
        }
    }
}
