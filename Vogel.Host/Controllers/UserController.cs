using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;
using Vogel.Host.Models;

namespace Vogel.Host.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        IAuthorizationService _auth;
        private readonly IMongoDbRepository<User> _userRepository;

        //public UserController(IMongoDbRepository<User> userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        private ISecurityContext securityContext;

        public UserController(ISecurityContext securityContext)
        {
            this.securityContext = securityContext;
        }

        [Route("")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        public async Task<IActionResult> ListAsync()
        {
            return Ok(securityContext.User);
        }
        //[Route("{id}")]
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        //public async Task<IActionResult> GetUser(string id)
        //{
        //    var filter = new FilterDefinitionBuilder<User>()
        //        .Eq(x => x.Id, id);

        //    var cursor = await _userRepository.AsMongoCollection().FindAsync(filter);

        //    var user = await cursor.FirstAsync();

        //    return Ok(user);
        //}

        //[Route("")]
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        //public async Task<IActionResult> CreateUser(UserModel model)
        //{
        //    var user = new User
        //    {
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        BirthDate = model.BirthDate,
        //        MediaId = model.MediaId
        //    };

        //    await _userRepository.InsertAsync(user);

        //    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        //}


    }
}
