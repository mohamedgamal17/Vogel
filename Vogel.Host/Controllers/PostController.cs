using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vogel.Application.CommentReactions.Commands;
using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.CommentReactions.Queries;
using Vogel.Application.Comments.Commands;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Queries;
using Vogel.Application.Common.Models;
using Vogel.Application.PostReactions.Commands;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.PostReactions.Queries;
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

        [Route("{postId}/reactions")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK , Type = typeof(Paging<PostReactionDto>))]
        public async Task<IActionResult> ListPostReactions(string postId , string? cursor = null, int limit = 10)
        {
            var query = new ListPostReactionQuery
            {
                PostId = postId,
                Cursor = cursor,
                Limit = limit
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("{postId}/reactions/{reactionId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostReactionDto))]
        public async Task<IActionResult> GetPostReaction(string postId , string reactionId)
        {
            var query = new GetPostReactionQuery
            {
                PostId = postId,
                Id = reactionId
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }


        [Route("{postId}/reactions")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostReactionDto))]
        public async Task<IActionResult> CreatePostReaction(string postId , [FromBody] CreatePostReactionCommand command)
        {
            var result = await Mediator.Send(command);

            return CreatedAtAction(nameof(GetPostReactionQuery), new { postId = postId, reactionId = result.Value?.Id });
        }

        [Route("{postId}/reactions/{reactionId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostReactionDto))]
        public async Task<IActionResult> UpdatePostReaction(string postId , string reactionId, [FromBody] UpdateCommentReactionCommand command)
        {
            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [Route("{postId}/reactions/{reactionId}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemovePostReaction(string postId , string reactionId)
        {
            var command = new RemovePostReactionCommand
            {
                PostId = postId,
                Id = reactionId
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

        [Route("{postId}/comments/{commentId}/subcomments")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CommentAggregateDto>))]
        public async Task<IActionResult> GetSubComments(string postId , string commentId , string? cursor = null, int limit = 10)
        {
            var query = new GetSubCommentsQuery
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
        public async Task<IActionResult> CreatePostComment(string postId ,[FromBody] CreateCommentCommand command)
        {
            command.SetPostId(postId);

            var result = await Mediator.Send(command);

            return CreatedAtAction(result, nameof(GetComment), new {postId = result.Value?.Id, commentId = result.Value?.Id});
        }

        [Route("{postId}/comments/{commentId}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentAggregateDto))]
        public async Task<IActionResult> UpdatePostComment(string postId , string commentId , [FromBody]UpdateCommentCommand command)
        {
            command.SetPostId(postId);

            command.SetId(commentId);

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

        [Route("{postId}/comments/{commentId}/reactions")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Paging<CommentReactionDto>))]
        public async Task<IActionResult> ListCommentReaction(string postId , string commentId, string? cursor = null, int limit = 10)
        {
            var query = new ListCommentReactionQuery
            {
                PostId = postId,
                CommentId = commentId,
                Cursor = cursor,
                Limit = limit
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }

        [Route("{postId}/comments/{commentId}/reactions/{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentReactionDto))]
        public async Task<IActionResult> GetCommmentReaction(string postId , string commentId , string id )
        {
            var query = new GetCommentReactionQuery
            {
                PostId = postId,
                CommentId = commentId,
                Id = id
            };

            var result = await Mediator.Send(query);

            return Ok(result);
        }



        [Route("{postId}/comments/{commentId}/reactions")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CommentReactionDto))]
        public async Task<IActionResult> CreateCommentReaction(string postId , string commentId , [FromBody] CreateCommentReactionCommand command)
        {
            command.SetCommentId(commentId);
            command.SetPostId(postId);

            var result = await Mediator.Send(command);

            return CreatedAtAction(result, nameof(CommentReactionDto), new { postId = postId, commentId = commentId, id = result.Value?.Id });
        }


        [Route("{postId}/comments/{commentId}/reactions/{id}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentReactionDto))]
        public async Task<IActionResult> UpdateCommentReaction(string postId , string commentId, string id, [FromBody] UpdateCommentReactionCommand command)
        {
            command.SetPostId(postId);
            command.SetCommentId(commentId);
            command.SetId(id);

            var result = await Mediator.Send(command);

            return Ok(result);
        }

        [Route("{postId}/comments/{commentId}/reactions/{id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveCommentReaction(string postId , string commentId , string id)
        {
            var command = new RemoveCommentReactionCommand
            {
                PostId = postId,
                CommentId = commentId,
                Id = id

            };

            var result = await Mediator.Send(command);

            return NoContent(result);
        }

    }
}
