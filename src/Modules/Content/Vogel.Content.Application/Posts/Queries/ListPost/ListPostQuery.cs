using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Posts.Dtos;
namespace Vogel.Content.Application.Posts.Queries.ListPost
{
    [Authorize]
    public class ListPostQuery : PagingParams, IQuery<Paging<PostDto>>
    {
    }
}
