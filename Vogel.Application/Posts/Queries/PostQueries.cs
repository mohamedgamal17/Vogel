using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Posts.Dtos;

namespace Vogel.Application.Posts.Queries
{
    public class ListPostPostQuery: PagingParams  ,IQuery<Paging<PostAggregateDto>>
    {

    }

    public class GetPostByIdQuery : IQuery<PostAggregateDto>
    {
        public string Id { get; set; }
    }


}
