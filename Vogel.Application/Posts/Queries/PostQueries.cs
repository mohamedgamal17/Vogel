using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Posts.Dtos;

namespace Vogel.Application.Posts.Queries
{
    public abstract class ListPostQueryBase : PagingParams, IQuery<Paging<PostAggregateDto>>
    { }

    public class ListPostQuery: ListPostQueryBase
    {

    }

    public class ListUserPostQuery : ListPostQueryBase
    {
        public string UserId { get; set; }
    }

    public class GetPostByIdQuery : IQuery<PostAggregateDto>
    {
        public string Id { get; set; }
    }

    public class GetUserPostById : IQuery<PostAggregateDto>
    {
        public string Id { get; set; }
        public string UserId { get; set; }

    }


}
