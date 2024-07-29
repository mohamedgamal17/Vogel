using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Commands;
using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Friendship.Queries;
using Vogel.Host.Models;
using Vogel.Host.Models.Friendship;
namespace Vogel.Host.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/friendRequests")]
    public class FriendRequestController : VogelController
    {
        public FriendRequestController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }


        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Paging<FriendRequestDto>>))]

        public async Task<IActionResult> ListFriendRequests(string? cursor = null, bool asending = false, int limit = 10)
        {
            var query = new ListFriendRequestQuery
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FriendRequestDto>))]
        public async Task<IActionResult> GetFriendRequest(string id)
        {
            var query = new GetFriendByIdQuery
            {
                Id = id
            };

            var result = await SendAsync(query);

            return Ok(result);
        }


        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<FriendRequestDto>))]
        public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestModel model)
        {
            var command = model.ToSendFriendRequestCommand();

            var result = await SendAsync(command);

            return CreatedAtAction(result, nameof(GetFriendRequest), new { id = result.Value?.Id });
        }


        [HttpPost]
        [Route("{id}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FriendDto>))]
        public async Task<IActionResult> AcceptFriendRequest(string id)
        {
            var command = new AcceptFriendRequestCommand { FriendRequestId = id };

            var result = await SendAsync(command);

            return Ok(result);
        }

        [HttpPost]
        [Route("{id}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FriendRequestDto>))]
        public async Task<IActionResult> RejectFriendRequest(string id)
        {
            var command = new RejectFriendRequestCommand { FriendRequestId = id };

            var result = await SendAsync(command);

            return Ok(result);
        }


        [HttpPost]
        [Route("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<FriendRequestDto>))]
        public async Task<IActionResult> CancelFriendRequest(string id)
        {
            var command = new CancelFriendRequestCommand { FriendRequestId = id };

            var result = await SendAsync(command);

            return Ok(result);
        }

    }
}
