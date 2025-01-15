using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;

namespace Vogel.Content.Application.Posts.Queries.GetPostbyId
{
    public class GetPostByIdQueryHandler : IApplicationRequestHandler<GetPostByIdQuery, PostDto>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public GetPostByIdQueryHandler(PostMongoRepository postMongoRepository, IPostResponseFactory postResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<PostDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.GetPostViewById(request.PostId);

            if(post == null)
            {
                return new Result<PostDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            return await _postResponseFactory.PreparePostDto(post!);
        }
    }
}
