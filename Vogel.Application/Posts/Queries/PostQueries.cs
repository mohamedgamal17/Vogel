using Vogel.Application.Common.Interfaces;
using Vogel.Application.Posts.Dtos;

namespace Vogel.Application.Posts.Queries
{
    public class ListPostPostQuery : IQuery<List<PostAggregateDto>>
    {

    }

    public class GetPostByIdQuery : IQuery<PostAggregateDto>
    {
        public string Id { get; set; }
    }


}
