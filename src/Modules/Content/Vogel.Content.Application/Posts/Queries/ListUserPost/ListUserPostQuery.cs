using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Posts.Dtos;
namespace Vogel.Content.Application.Posts.Queries.ListUserPost
{
    [Authorize]
    public class ListUserPostQuery : PagingParams , IQuery<Paging<PostDto>>
    {
        public string UserId { get; set; }
    }
}
