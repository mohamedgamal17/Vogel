using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Queries;
using Vogel.Host.Models;
namespace Vogel.Host.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : VogelController
    {
        public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        public async Task<IActionResult> GetCurrentUser()
        {
            var query = new GetCurrentUserQuery();

            var result = await SendAsync(query);

            return Ok(result);
        }


        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
        public async Task<IActionResult> CreateUser(UserModel model)
        {
            var command = model.ToCreateUserCommand();

            var result = await SendAsync(command);

            return CreatedAtAction(result, nameof(GetCurrentUser));
        }

        [Route("")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
        public async Task<IActionResult> UpdateUser(UserModel model)
        {
            var command = model.ToUpdateUserCommand();

            var result = await SendAsync(command);

            return Ok(result);
        }
    }
}
