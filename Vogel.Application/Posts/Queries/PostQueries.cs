
using Vogel.Application.Common.Models;
using Vogel.Application.Posts.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;
namespace Vogel.Application.Posts.Queries
{
    public abstract class ListPostQueryBase : PagingParams, IQuery<Paging<PostDto>>
    { }

    public class ListPostQuery: ListPostQueryBase
    {

    }

    public class ListUserPostQuery : ListPostQueryBase
    {
        public string UserId { get; set; }
    }

    public class GetPostByIdQuery : IQuery<PostDto>
    {
        public string Id { get; set; }
    }

    public class GetUserPostById : IQuery<PostDto>
    {
        public string Id { get; set; }
        public string UserId { get; set; }

    }


}
