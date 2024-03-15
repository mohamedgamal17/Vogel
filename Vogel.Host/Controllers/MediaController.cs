using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Medias.Commands;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Medias.Queries;
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

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MediaAggregateDto>))]
        public async Task<IActionResult> ListMedias()
        {
            var query = new ListMediaQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Route("{mediaId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MediaAggregateDto))]
        public async Task<IActionResult> GetMedia(string mediaId)
        {
            var query = new GetMediaByIdQuery { Id = mediaId };

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MediaAggregateDto))]
        public async Task<IActionResult> CreateMedia([FromForm]MediaModel model)
        {
            var command = await model.ToCreateMediaCommand();

            var result = await Mediator.Send(command);

            return CreatedAtAction(result, nameof(GetMedia), new { mediaId = result.Value!.Id });
        }


        [Route("{mediaId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveMedia(string mediaId)
        {
            var command = new RemoveMediaCommand { Id = mediaId };

            var result = await Mediator.Send(command);

            return NoContent(result);
        }
    }
}
