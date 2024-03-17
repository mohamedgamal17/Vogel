using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Common.Models;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Queries;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserAggregateDto>))]
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

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<UserAggregateDto>))]
        public async Task<IActionResult> GetAsync(string id)
        {
            var query = new GetUserByIdQuery { Id = id };

            var result = await SendAsync(query);

            return Ok(result);
        }
    }
}
