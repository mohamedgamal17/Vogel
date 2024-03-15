﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Posts.Commands;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Queries;
using Vogel.Host.Models;

namespace Vogel.Host.Controllers
{
    [Route("api/posts")]
    [ApiController]
    [Authorize]
    public class PostController : VogelController
    {
        public PostController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<PostAggregateDto>>))]
        public async Task<IActionResult> ListPostAsync()
        {
            var query = new ListPostPostQuery();

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("{postId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PostAggregateDto>))]
        public async Task<IActionResult> GetPost(string postId)
        {
            var query = new GetPostByIdQuery { Id = postId };

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<PostAggregateDto>))]
        public async Task<IActionResult> CreatePost([FromBody]PostModel model)
        {
            var command = model.ToCreatePostCommand();

            var result = await Mediator.Send(command);

            return CreatedAtAction(result, nameof(GetPost), new { postId = result.Value!.Id });
        }


        [Route("{postId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PostDto>))]
        public async Task<IActionResult> UpdatePost(string postId , [FromBody] PostModel model)
        {
            var command = model.ToUpdatePostCommand(postId);

            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [Route("{postId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemovePost(string postId)
        {
            var command = new RemovePostCommand
            {
                Id = postId
            };

            var result = await Mediator.Send(command);

            return NoContent(result);
        }

    }
}
