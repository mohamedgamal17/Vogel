using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Commands;
using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Friendship.Queries;
using Vogel.Domain.Friendship;
using Vogel.Host.Models;

namespace Vogel.Host.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/friends")]
    public class FriendController : VogelController
    {
        public FriendController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Paging<Friend>>))]
        public async Task<IActionResult> ListUserFriend(string? cursor =null , bool asending = false, int limit = 10)
        {
            var query = new ListFriendQuery
            {
                Cursor = cursor,
                Asending = asending,
                Limit = limit
            };

            var result = await SendAsync(query);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Friend>))]
        public async Task<IActionResult> GetFriend(string id)
        {
            var query = new GetFriendByIdQuery { Id = id };

            var result = await SendAsync(query);

            return Ok(result);
        }


        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveFriend(string id)
        {
            var query = new RemoveFriendCommand { Id = id };

            var result = await SendAsync(query);

            return NoContent();
        }
    }
}
