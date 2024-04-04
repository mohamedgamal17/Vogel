using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.Comments.Commands;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Queries;
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
        public async Task<IActionResult> ListPostAsync(string? cursor = null , int limit = 10)
        {
            var query = new ListPostQuery()
            {
                Cursor = cursor,
                Limit = limit
            };

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PostAggregateDto>))]
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

        [Route("{postId}/comments")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(ApiResponse<CommentAggregateDto>))]
        public async Task<IActionResult> ListPostComments(string postId , string? cursor = null, int limit = 10)
        {
            var query = new ListCommentsQuery
            {
                PostId = postId,
                Cursor = cursor,
                Limit = limit
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("{postId}/comments/{commentId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CommentAggregateDto>))]
        public async Task<IActionResult> GetComment(string postId, string commentId)
        {
            var query = new GetCommentQuery
            {
                PostId = postId,
                CommentId = commentId
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("{postId}/comments")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommentAggregateDto))]
        public async Task<IActionResult> CreatePostComment(string postId , CommentModel model)
        {
            var command = new CreateCommentCommand
            {
                Content = model.Content,
                PostId = postId
            };

            var result = await Mediator.Send(command);

            return CreatedAtAction(result, nameof(GetComment), new {postId = result.Value?.Id, commentId = result.Value?.Id});
        }

        [Route("{postId}/comments/{commentId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDto))]
        public async Task<IActionResult> UpdatePostComment(string postId , string commentId , CommentModel model)
        {
            var command = new UpdateCommentCommand
            {
                Id = commentId,
                PostId = postId,
                Content = model.Content
            };

            var result = await Mediator.Send(command);

            return Ok(result);
       
        }

        [Route("{postId}/comments/{commentId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemovePostComment(string postId, string commentId)
        {
            var command = new RemoveCommentCommand
            {
                Id = commentId,
                PostId = postId,
            };

            var result = await Mediator.Send(command);

            return NoContent(result);
        }

    }
}
