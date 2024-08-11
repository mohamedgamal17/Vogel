using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Posts.Queries;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Queries;
using Vogel.MongoDb.Entities.Common;
namespace Vogel.Host.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/people")]
    public class PeopleController : VogelController
    {
        public PeopleController(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {

        }

        [Route("")]
        [HttpGet]  
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserDto>))]
        public async Task<IActionResult> ListAsync(string? cursor = null, bool asending = false, int limit = 10)
        {
            var query = new ListUserQuery
            {
                Asending = asending,
                Limit = limit,
                Cursor = cursor
            };

            var result = await SendAsync(query);

            return Ok(result);
        }

        [Route("search/{name}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserDto>))]

        public async Task<IActionResult> SearchOnUsers(string name , string? cursor = null, bool asending = false, int limit = 10)
        {
            var query = new SearchOnUserByNameQuery
            {
                Name = name,
                Cursor = cursor,
                Asending = asending,
                Limit = limit
            };

            var result = await SendAsync(query);

            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserDto>))]
        public async Task<IActionResult> GetAsync(string id)
        {
            var query = new GetUserByIdQuery { Id = id };

            var result = await SendAsync(query);

            return Ok(result);
        }

        [Route("{userId}/posts")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserDto>))]
        public async Task<IActionResult> GetUserPosts(string userId, string? cursor = null, bool asending = false, int limit = 10)
        {
            var query = new ListUserPostQuery
            {
                UserId = userId,
                Cursor = cursor,
                Asending = asending,
                Limit = limit
            };

            var result = await SendAsync(query);

            return Ok(result);
        }

        [Route("{userId}/posts/{postId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserDto>))]
        public async Task<IActionResult> GetUserPost(string userId , string postId)
        {
            var query = new GetUserPostById
            {
                UserId = userId,
                Id = postId
            };

            var result = await SendAsync(query);

            return Ok(result);
        }
    }
}
