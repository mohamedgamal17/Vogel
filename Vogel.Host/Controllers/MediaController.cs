using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Medias.Dtos;
using Vogel.Host.Models;
namespace Vogel.Host.Controllers
{
    [Route("api/medias")]
    [ApiController]
    [Authorize]
    public class MediaController : VogelController
    {
        public MediaController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [Route("{mediaId}")]

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MediaAggregateDto))]
        public async Task<IActionResult> GetMedia(string mediaId)
        {
            throw new NotImplementedException();
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MediaAggregateDto))]
        public async Task<IActionResult> CreateMedia([FromForm]MediaModel model)
        {
            var command = await model.ToCreateMediaCommand();

            var result = await Mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { Message = result.Exception!.GetType().Name });
            }

            var response = new ApiResponse<MediaAggregateDto>
            {
                Data = result.Value!
            };

            return CreatedAtAction(nameof(GetMedia), new { mediaId = result.Value!.Id }, response);
        }
    }
}
